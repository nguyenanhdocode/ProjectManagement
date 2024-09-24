import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { useAsyncError, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Box, FormControl } from '@mui/material';
import { DatePicker, DateTimePicker } from '@mui/x-date-pickers';
import * as yup from "yup"
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import CustomDateTimePicker from '../Common/CustomDateTimePicker';
import * as projectValidatorConfig from './ProjectValidatorConfiguration';
import { endpoints } from '../../Config/API';
import { APIContext, NotificationContext } from '../../Common/Contexts';
import { useContext, useEffect, useState } from 'react';
import { LoadingButton } from '@mui/lab';
import { convertToUTC, convertUTCDateToLocalDate, formatDateTime } from '../../Utils/DayHelper';
import dayjs from 'dayjs';

export function UpdateProjectForm({
    onCancel, onSuccess, projectId
}) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [project, setProject] = useState(null);
    const [isBeginDateChanged, setBeginDateChanged] = useState(false);
    const [isTaskAffected, setTaskAffected] = useState(false);

    const schema = yup.object({
        name: yup
            .string()
            .required(t('project.name.required'))
            .max(
                projectValidatorConfig.NameMaxLength,
                t('user.name.maxLength').replace('{0}', projectValidatorConfig.NameMaxLength)
            ),
        managerId: yup.string()
    }).required();

    const {
        control,
        register,
        handleSubmit,
        watch,
        setError,
        trigger,
        setValue,
        reset,
        formState: { errors, isValidating },
    } = useForm({ resolver: yupResolver(schema) });
    //#endregion

    //#region Methods
    async function onSubmit(data) {

        try {
            setProcessing(true);
            const url = endpoints['updateProject'].replace('{0}', project['id']);
            const res = await bearerRequest.put(url, data);
            notificationManager.showSuccess(t('project.updated.success'));

            setProcessing(false);

            if (typeof onSuccess === 'function')
                onSuccess();
        } catch (error) {
            setProcessing(false);
        }
    }

    async function getProject(id) {
        try {
            const res = await bearerRequest.get(endpoints['getProject'].replace('{0}', id));
            const data = res.data.result;

            setValue('name', data['name'])
            setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(data['beginDate']))));
            setValue('endDate', dayjs(convertUTCDateToLocalDate(new Date(data['endDate']))));
            setValue('description', data['description']);
            setValue('managerId', data['managerId']);

            setProject(data);
        } catch (error) {

        }
    }


    async function onBeginDateChange(value) {
        // const isAffected = await !checkBeforeUpdateBeginDate(projectId, value.toISOString());
        let isChanged = value != convertUTCDateToLocalDate(new Date(project['beginDate']));
        setBeginDateChanged(isChanged);
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getProject(projectId);
    }, []);
    //#endregion

    return <>
        <Dialog open={true} PaperProps={{ component: 'form' }}
            maxWidth='sm' fullWidth={true} onSubmit={handleSubmit(onSubmit)}
            sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('project.update')}</DialogTitle>

            <DialogContent>

                <TextField {...register('name')} variant='standard' label={t('common.name')}
                    fullWidth error={Boolean(errors?.name)} helperText={errors?.name?.message}
                    InputLabelProps={{ shrink: true }}>
                </TextField>

                {/* <CustomDateTimePicker control={control} name='beginDate'
                    error={Boolean(errors?.beginDate)} label={t('common.beginDate')}
                    helperText={errors?.beginDate?.message} variant='standard'
                    required={true} labelShink={true} customOnChange={onBeginDateChange}>

                </CustomDateTimePicker>

                <CustomDateTimePicker control={control} name='endDate'
                    error={Boolean(errors?.endDate)} label={t('common.endDate')}
                    helperText={errors?.endDate?.message} variant='standard'
                    labelShink={true}>

                </CustomDateTimePicker> */}

                <TextField multiline margin="dense" label={t('common.description')}
                    fullWidth variant="standard" {...register('description')}
                    InputLabelProps={{ shrink: true }} />

                <input type='hidden' {...register('managerId')} />
            </DialogContent>

            <DialogActions>
                <Button onClick={onCancel}>{t('common.cancel')}</Button>
                <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
            </DialogActions>

        </Dialog>
    </>
}