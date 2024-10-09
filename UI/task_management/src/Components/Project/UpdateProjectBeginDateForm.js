import { Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle, Paper, Typography } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import { LoadingButton } from "@mui/lab";
import * as yup from "yup"
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import CustomDateTimePicker from "../Common/CustomDateTimePicker";
import { endpoints } from "../../Config/API";
import dayjs from "dayjs";
import { convertToUTC, convertUTCDateToLocalDate } from "../../Utils/DayHelper";

export default function UpdateProjectBeginDateForm({
    onCancel, onSuccess, projectId
}) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [project, setProject] = useState(null);
    const [isAffected, setAffected] = useState(false);

    const schema = yup.object({
        beginDate: yup.date()
            .required(t('project.beginDate.required'))
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
            const url = endpoints['updateProjectBeginDate'].replace('{0}', project['id']);
            const res = await bearerRequest.put(url, data);
            notificationManager.showSuccess(t('project.updated.success'));

            setProcessing(false);

            if (typeof onSuccess === 'function')
                onSuccess();
        } catch (error) {
            setProcessing(false);
        }
    }

    async function onBeginDateChange(value) {
        try {
            let url = endpoints['checkBeforeUpdateProjectBeginDate'].replace('{0}', projectId);
            url = `${url}?beginDate=${new Date(convertToUTC(value)).toISOString()}`
            const res = await bearerRequest.head(url);
            setAffected(false);
        } catch (error) {
            setAffected(true);
        }
    }

    async function getProject(id) {
        try {
            const res = await bearerRequest.get(endpoints['getProject'].replace('{0}', id));
            const data = res.data.result;

            setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(data['beginDate']))));

            setProject(data);
        } catch (error) {

        }
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
                {isAffected && <Alert severity="warning">
                    <Typography>{t('project.taskAffected.alert')}</Typography>
                    <Button sx={{ mt: 1 }} variant="outlined" size="small">Xem chi tiết</Button>
                </Alert>}
                <CustomDateTimePicker control={control} name='beginDate'
                    error={Boolean(errors?.beginDate)} label={t('common.beginDate')}
                    helperText={errors?.beginDate?.message} variant='filled'
                    required={true} labelShink={true} customOnClose={onBeginDateChange}>

                </CustomDateTimePicker>
            </DialogContent>

            <DialogActions>
                <Button onClick={onCancel}>{t('common.cancel')}</Button>
                <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
            </DialogActions>
        </Dialog>
    </>
}