import { yupResolver } from "@hookform/resolvers/yup";
import { useContext, useEffect, useRef, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext, UserContext } from "../../Common/Contexts";
import * as yup from "yup"
import { Autocomplete, Avatar, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, Grid, InputLabel, Menu, MenuItem, Select, TextField, Typography } from "@mui/material";
import CustomDateTimePicker from "../Common/CustomDateTimePicker";
import { LoadingButton } from "@mui/lab";
import { endpoints } from "../../Config/API";
import * as taskValidatorConfig from '../Task/TaskValidatorConfiguration';
import dayjs from "dayjs";
import { convertUTCDateToLocalDate, formatDateTime } from "../../Utils/DayHelper";
import { ThreeSixty } from "@mui/icons-material";

export default function UpdateSubtaskForm({ onCancel, onSuccess, taskId, parentId }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [tasks, setTasks] = useState(null);
    const [prevTask, setPrevTask] = useState(null);
    const { user, userDispatch } = useContext(UserContext);
    const [parent, setParent] = useState();
    const [task, setTask] = useState({});
    const [allowedNewStatus, setAllowedNewStatus] = useState([]);

    const schema = yup.object({
        name: yup
            .string()
            .required(t('task.name.required'))
            .max(
                taskValidatorConfig.NameMaxLength,
                t('task.name.maxLength').replace('{0}', taskValidatorConfig.NameMaxLength)
            ),
        beginDate: yup.date()
            .required(t('task.beginDate.required'))
            .test({
                name: 'mustLater',
                exclusive: false,
                test: function (value) {
                    if (value < convertUTCDateToLocalDate(new Date(parent['beginDate'])))
                        return this.createError({
                            message: t('task.beginDate.mustLater')
                                .replace('{0}', formatDateTime(convertUTCDateToLocalDate(new Date(parent['beginDate']))))
                        })
                    return true;
                }
            })
            .test({
                name: 'mustBefore',
                exclusive: false,
                test: function (value) {
                    if (value >= new Date(watch('endDate')))
                        return this.createError({
                            message: t('task.beginDate.mustBefore')
                                .replace('{0}', formatDateTime(new Date(watch('endDate'))))
                        })
                    return true;
                }
            }),
        endDate: yup.date()
            .required(t('task.endDate.required'))
            .test({
                name: 'mustLater',
                exclusive: false,
                message: t('task.endDate.mustLater'),
                test: function (value) {
                    return value >= new Date(watch('beginDate'))
                }
            })
            .test({
                name: 'mustBefore',
                exclusive: false,
                test: function (value) {
                    if (value > convertUTCDateToLocalDate(new Date(parent?.endDate)))
                        return this.createError({ message: t('task.endDate.mustBefore')
                            .replace('{0}', formatDateTime(convertUTCDateToLocalDate(new Date(parent?.endDate))))});
                    return true;
                }
            }),
        assignedToUserId: yup.string().required(),
        status: yup.string().required(),
    }).required();

    const {
        control,
        register,
        handleSubmit,
        setValue,
        watch,
        setError,
        trigger,
        formState: { errors, isValidating },
    } = useForm({ resolver: yupResolver(schema) });

    //#endregion

    //#region Methods
    async function onSubmit(data) {
        setProcessing(true);
        try {
            const res = await bearerRequest.put(endpoints['updateSubtask'].replace('{0}', taskId), data);
            notificationManager.showSuccess(t('task.updated.success'));
            setProcessing(false);
            onSuccess();
        } catch (error) {
            setProcessing(false);
        }
    }

    async function getTask(id) {
        try {
            const res = await bearerRequest.get(endpoints['getTask'].replace('{0}', id));
            const data = res.data.result;
            setTask(data);

            setValue('name', data['name']);
            setValue('note', data['note']);
            setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(data?.beginDate))));
            setValue('endDate', dayjs(convertUTCDateToLocalDate(new Date(data?.endDate))));
            setValue('status', parseInt(data['status']));

        } catch (error) {

        }
    }

    async function getParent(id) {
        try {
            const res = await bearerRequest.get(endpoints['getTask'].replace('{0}', id));
            const data = res.data.result;
            setParent(data);

        } catch (error) {

        }
    }

    async function getAllowedNewStatus() {
        try {
            let url = endpoints['getAllowedNewStatus'].replace('{0}', taskId);
            const res = await bearerRequest.get(url);
            const data = res.data.result;
            setAllowedNewStatus(data);
        } catch (error) {
        }
    }

    //#endregion

    //#region Hooks
    useEffect(() => {
        getParent(parentId);
        getTask(taskId);
        getAllowedNewStatus();
        setValue('assignedToUserId', user['id']);
        setValue('status', parseInt(task['status']));
    }, []);
    //#endregion

    return <>
        <Dialog open={true}
            maxWidth='sm' fullWidth={true} sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('task.update')}</DialogTitle>
            {task && <Box component='form' onSubmit={handleSubmit(onSubmit)} noValidate>
                <DialogContent sx={{ pt: 0 }}>
                    <TextField {...register('name')} variant='filled' label={t('common.name')}
                        fullWidth error={Boolean(errors?.name)} helperText={errors?.name?.message}
                        required InputLabelProps={{ shrink: true }} ></TextField>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Grid container>
                            <Grid xs={12} sx={6} md={6} lg={6}>
                                <Box sx={{ pr: 1 }}>
                                    <CustomDateTimePicker control={control} name='beginDate'
                                        error={Boolean(errors?.beginDate)} label={t('common.beginDate')}
                                        helperText={errors?.beginDate?.message} variant='filled'
                                        required={true} defaultValue={convertUTCDateToLocalDate(new Date())}
                                        labelShink={true}></CustomDateTimePicker>
                                </Box>
                            </Grid>
                            <Grid xs={12} sx={6} md={6} lg={6}>
                                <Box sx={{ pl: 1 }}>
                                    <CustomDateTimePicker control={control} name='endDate'
                                        error={Boolean(errors?.endDate)} label={t('common.endDate')}
                                        helperText={errors?.endDate?.message} variant='filled'
                                        labelShink={true}
                                        defaultValue={(() => {
                                            let date = new Date(task['beginDate']);
                                            date.setDate(date.getDate() + 1);
                                            return convertUTCDateToLocalDate(date);
                                        })()}
                                    ></CustomDateTimePicker>
                                </Box>
                            </Grid>
                        </Grid>
                    </Box>

                    <Controller control={control} name="status" render={({ field: {
                        onChange, value, ...rest
                    } }) => (
                        <FormControl variant="filled" sx={{ mt: 2 }} fullWidth>
                            <InputLabel id="demo-simple-select-filled-label" shrink required>
                                {t('common.status')}
                            </InputLabel>
                            <Select onChange={onChange} value={value} {...rest}
                                labelId="demo-simple-select-filled-label"
                                id="demo-simple-select-filled">

                                {allowedNewStatus.map(p => <MenuItem value={p}>
                                    {t(taskValidatorConfig.TaskStatusLang[p])}
                                </MenuItem>)}
                            </Select>
                        </FormControl>
                    )}></Controller>

                    <input type="hidden" {...register('assignedToUserId')}></input>

                    <TextField multiline autoFocus margin="dense" label={t('common.note')}
                        fullWidth variant="filled" {...register('note')} InputLabelProps={{ shrink: true }}
                        />
                </DialogContent>
                <DialogActions>
                    <Button onClick={onCancel}>{t('common.cancel')}</Button>
                    <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
                </DialogActions>
            </Box>}
        </Dialog>
    </>
}