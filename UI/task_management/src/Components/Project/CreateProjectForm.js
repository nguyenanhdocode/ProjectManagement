import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { useNavigate } from 'react-router-dom';
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

export default function CreateProjectForm({
    onCancel, onSuccess
}) {
    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);

    const schema = yup.object({
        name: yup
            .string()
            .required(t('project.name.required'))
            .max(
                projectValidatorConfig.NameMaxLength,
                t('user.name.maxLength').replace('{0}', projectValidatorConfig.NameMaxLength)
            ),
        beginDate: yup.date()
            .required(t('project.beginDate.required')),
        endDate: yup.date()
            .required(t('project.endDate.required'))
            .test({
                name: 'endDateConstraint',
                exclusive: false,
                message: t('project.endDate.valid'),
                test: function (value) {
                    return value > watch('beginDate')
                }
            }),
        managerId: yup.string()
    }).required();

    const {
        control,
        register,
        handleSubmit,
        watch,
        setError,
        trigger,
        reset,
        formState: { errors, isValidating },
    } = useForm({ resolver: yupResolver(schema) });
    //#endregion

    //#region Methods
    async function onSubmit(data) {
        try {
            setProcessing(true);
            const res = await bearerRequest.post(endpoints['createProject'], data);
            notificationManager.showSuccess(t('project.created.success'));
            setProcessing(false);

            if (typeof onSuccess === 'function')
                onSuccess();
        } catch (error) {
            setProcessing(false);
        }
    }

    //#endregion

    //#region Hooks

    //#endregion

    return <>
        <Dialog open={true} PaperProps={{ component: 'form' }}
            maxWidth='sm' fullWidth={true} onSubmit={handleSubmit(onSubmit)}
            sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('project.create')}</DialogTitle>

            <DialogContent>

                <TextField {...register('name')} variant='filled' label={t('common.name')}
                    fullWidth error={Boolean(errors?.name)} helperText={errors?.name?.message}
                    InputLabelProps={{ shrink: true }}>
                </TextField>

                <CustomDateTimePicker control={control} name='beginDate'
                    error={Boolean(errors?.beginDate)} label={t('common.beginDate')}
                    helperText={errors?.beginDate?.message} variant='filled'
                    required={true} labelShink={true}>

                </CustomDateTimePicker>

                <CustomDateTimePicker control={control} name='endDate'
                    error={Boolean(errors?.endDate)} label={t('common.endDate')}
                    helperText={errors?.endDate?.message} variant='filled'
                    labelShink={true}>

                </CustomDateTimePicker>

                <TextField multiline margin="dense" label={t('common.description')}
                    fullWidth variant="filled" {...register('description')}
                    InputLabelProps={{ shrink: true }}/>

                <input type='hidden' {...register('managerId')} />
            </DialogContent>

            <DialogActions>
                <Button onClick={onCancel}>{t('common.cancel')}</Button>
                <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
            </DialogActions>

        </Dialog>
    </>
}