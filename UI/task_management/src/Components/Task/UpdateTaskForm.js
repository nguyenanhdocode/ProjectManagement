import { yupResolver } from "@hookform/resolvers/yup";
import { useContext, useEffect, useRef, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import * as yup from "yup"
import { Alert, Autocomplete, Avatar, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, Grid, InputLabel, Menu, MenuItem, Select, TextField, Typography } from "@mui/material";
import CustomDateTimePicker from "../Common/CustomDateTimePicker";
import { LoadingButton } from "@mui/lab";
import { endpoints } from "../../Config/API";
import * as taskValidatorConfig from '../Task/TaskValidatorConfiguration';
import dayjs from "dayjs";
import { convertToUTC, convertUTCDateToLocalDate, formatDateTime } from "../../Utils/DayHelper";
import ConfirmDialog from "../Common/ConfirmDialog";
import Remain from "../Common/Remain";

export default function UpdateTaskForm({ onCancel, onSuccess, taskId }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [members, setMembers] = useState([]);
    const [assignedToUserId, setAssignedToUserId] = useState(null);
    const [tasks, setTasks] = useState(null);
    const [prevTask, setPrevTask] = useState(null);
    const [project, setProject] = useState({});
    const [task, setTask] = useState(null);
    const [isAffected, setAffected] = useState(false);
    const [allowedNewStatus, setAllowedNewStatus] = useState([]);
    const [canShorten, setCanShorten] = useState(false);
    const [isUpdated, setUpdated] = useState(false);

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
                test: function (value, context) {
                    if (!watch('previousTaskId') && value < convertUTCDateToLocalDate(new Date(project['beginDate']))) {
                        return this.createError({
                            message: `${t('task.beginDate.mustLater')}`.replace('{0}',
                                formatDateTime(convertUTCDateToLocalDate(new Date(project['beginDate'])))
                            )
                        })
                    }
                    return true;
                }
            })
            .test({
                name: 'mustLaterPrevTask',
                exclusive: false,
                test: function (value, contexts) {
                    if (!watch('previousTaskId'))
                        return true;

                    const task = tasks.find(p => p['id'] == watch('previousTaskId'));

                    if (value < convertUTCDateToLocalDate(new Date(task['endDate']))
                        && task['doneDate']
                        && value < convertUTCDateToLocalDate(new Date(task['doneDate']))) {
                        return this.createError({
                            message: `${t('task.beginDate.mustLater')}`.replace('{0}',
                                formatDateTime(convertUTCDateToLocalDate(new Date(task['endDate'])))
                            )
                        })
                    }

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
                    if (value > convertUTCDateToLocalDate(new Date(project['endDate']))) {
                        return this.createError({
                            message: `${t('task.endDate.mustBefore')}`.replace('{0}',
                                formatDateTime(convertUTCDateToLocalDate(new Date(project['endDate'])))
                            )
                        })
                    }

                    return true;
                }
            }),
        assignedToUserId: yup.string().required(t('task.assignToUser.required')),
        projectId: yup.string().required(),
        previousTaskId: yup.string().nullable(),
        status: yup.string().required()
            .test({
                name: 'invalidStatus',
                exclusive: false,
                test: function (value) {
                    if (value == taskValidatorConfig.DOING && new Date(watch('beginDate')) > new Date())
                        return this.createError({
                            message: t('task.cannotStart').replace('{0}', formatDateTime(new Date(watch('beginDate'))))
                        });

                    return true;
                }
            })
            .test({
                name: 'prevTaskMustDone',
                exclusive: false,
                test: async function (value) {
                    const prevTaskid = watch('previousTaskId');
                    if (!prevTaskid)
                        return true;

                    try {
                        const res = await bearerRequest.get(endpoints['getTask'].replace('{0}', prevTaskid));
                        const data = res.data.result;

                        if (!([taskValidatorConfig.DONE, taskValidatorConfig.REDO].includes(data['status']))
                            && value == taskValidatorConfig.DOING)
                            return this.createError({ message: t('task.prevTask.mustDone').replace('{0}', data['name']) });
                        return true;
                    } catch (error) {

                    }
                }
            }),
        note: yup.string().nullable()
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
            const res = await bearerRequest.put(endpoints['updateTask'].replace('{0}', taskId), data);
            notificationManager.showSuccess(t('task.updated.success'));
            setProcessing(false);

            const totalMinutes = (convertUTCDateToLocalDate(new Date(task['endDate'])) - new Date()) / (1000 * 60);

            if (watch('status') == taskValidatorConfig.DONE
                && totalMinutes >= 30
                && await checkCanUpdateBeginDate()) {
                setCanShorten(true);
            } else {
                onSuccess();
            }

        } catch (error) {
            setProcessing(false);
        }
    }

    async function getMembers(projectId) {
        try {
            const res = await bearerRequest.get(endpoints['getProjectMembers'].replace('{0}', projectId));
            setMembers(res['data']['result']);
        } catch (error) {

        }
    }

    async function getTasks(projectId) {
        try {
            let url = endpoints['getAvaiablePrevTask'].replace('{0}', projectId);
            url = `${url}?taskId=${taskId}`;
            const res = await bearerRequest.get(url);
            const data = res.data.result;
            setTasks(data);
        } catch (error) {

        }
    }

    function onPrevTaskChange(e, value) {
        setPrevTask(value);
        if (value?.endDate)
            setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(value?.endDate))));
    }

    async function getProject(id) {
        try {
            const res = await bearerRequest.get(endpoints['getProject'].replace('{0}', id));
            const data = res.data.result;
            setProject(data);

            setValue('projectId', data['id']);
        } catch (error) {

        }
    }

    async function getTask(id) {
        try {
            const res = await bearerRequest.get(endpoints['getTask'].replace('{0}', id));
            const data = res.data.result;
            setTask(data);

            setValue('name', data['name']);
            setValue('previousTaskId', data['previousTaskId']);
            setValue('endDate', dayjs(convertUTCDateToLocalDate(new Date(data['endDate']))));
            setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(data['beginDate']))));
            setValue('assignedToUserId', data['assignedToUser']['id']);
            setValue('note', data['note']);
            setValue('status', parseInt(data['status']));
        } catch (error) {

        }
    }

    async function onEndDateChange(value) {
        if (!value)
            return;

        try {
            let url = endpoints['checkAffectBeforeUpdateTaskEndDate'].replace('{0}', taskId);
            url = `${url}?endDate=${new Date(convertToUTC(value)).toISOString()}`
            const res = await bearerRequest.head(url);
            setAffected(false);
        } catch (error) {
            setAffected(true);
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

    async function onShortenDialogAgree() {
        try {
            const res = await bearerRequest.put(endpoints['shiftTask'].replace('{0}', taskId), {
                milliseconds: new Date() - new Date(task['endDate'])
            });
            onSuccess();
        } catch (error) {

        }
    }

    function onShortenDialogCancel() {
        onSuccess();
    }

    async function checkCanUpdateBeginDate() {
        try {
            let url = endpoints['checkCanUpdateBeginDate'].replace('{0}', taskId);
            const res = await bearerRequest.head(url);
            return true;
        } catch (error) {
            return true;
        }
    }

    //#endregion

    //#region Hooks
    useEffect(() => {
        getTask(taskId);
    }, []);

    useEffect(() => {
        if (!task)
            return;
        getProject(task['projectId']);
        getMembers(task['projectId']);
        getTasks(task['projectId']);
        getAllowedNewStatus();
    }, [task]);

    // useEffect(() => {
    //     if (isUpdated) {
    //         checkCanBeforeUpdateEndDate(); 
    //     }
    // }, [isUpdated]);
    //#endregion

    return <>
        {task && <Dialog open={true}
            maxWidth='sm' fullWidth={true} sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('task.update')}</DialogTitle>
            {console.log(errors)}
            <Box component='form' onSubmit={handleSubmit(onSubmit)} noValidate>
                <DialogContent sx={{ pt: 0 }}>
                    {isAffected && <Alert severity="warning">
                        <Typography>{t('project.taskAffected.alert')}</Typography>
                        <Button sx={{ mt: 1 }} variant="outlined" size="small">Xem chi tiết</Button>
                    </Alert>}

                    <TextField {...register('name')} variant='filled' label={t('common.name')}
                        fullWidth error={Boolean(errors?.name)} helperText={errors?.name?.message}
                        required InputLabelProps={{ shrink: true }}></TextField>

                    <Controller control={control} name="previousTaskId"
                        render={({ field: { onChange, value } }) => (
                            <Autocomplete
                                options={tasks || {}}
                                getOptionLabel={p => {
                                    var task = tasks?.find((v) => v['id'] === p);

                                    if (!task)
                                        return null;

                                    return `${task['name']}`;
                                }}
                                onChange={(e, value) => {
                                    onChange(value?.id);
                                    onPrevTaskChange(e, value);
                                }}
                                value={value || null}
                                renderOption={(props, option) => {
                                    return <MenuItem {...props}>
                                        <Box>
                                            <Typography color='text.disabled' fontSize='small'>{option['id']}</Typography>
                                            <Typography>{option['name']}</Typography>
                                            <Typography color='text.disabled' fontSize='small'>
                                                {formatDateTime(convertUTCDateToLocalDate(new Date(option['beginDate'])))} -
                                                {formatDateTime(convertUTCDateToLocalDate(new Date(option['endDate'])))}
                                            </Typography>
                                        </Box>
                                    </MenuItem>
                                }}
                                renderInput={(params) => (
                                    <TextField sx={{ mt: 2 }} variant="filled" fullWidth {...params}
                                        label={t('task.previousTask')} InputLabelProps={{ shrink: true }} />
                                )}
                            />
                        )}>
                    </Controller>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Grid container>
                            <Grid xs={12} sx={6} md={6} lg={6}>
                                <Box sx={{ pr: 1 }}>
                                    <CustomDateTimePicker control={control} name='beginDate'
                                        error={Boolean(errors?.beginDate)} label={t('common.beginDate')}
                                        helperText={errors?.beginDate?.message} variant='filled'
                                        required={true} defaultValue={convertUTCDateToLocalDate(new Date(project['beginDate']))}
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
                                            let date = new Date(project['beginDate']);
                                            date.setDate(date.getDate() + 1);
                                            return convertUTCDateToLocalDate(date);
                                        })()}
                                        customOnClose={onEndDateChange}
                                    ></CustomDateTimePicker>
                                </Box>
                            </Grid>
                        </Grid>
                    </Box>

                    {project['isManager'] ? <Controller control={control} name="assignedToUserId"
                        render={({ field: { onChange, value } }) => (
                            <Autocomplete
                                disablePortal
                                options={members}
                                getOptionLabel={p => {
                                    var member = members.find((v) => v['id'] === p);

                                    if (!member)
                                        return null;

                                    return `${member['firstName']} ${member['lastName']} (${member['email']})`;
                                }}
                                onChange={(e, value) => onChange(value?.id)}
                                value={value || null}
                                renderOption={(props, option) => {
                                    return <MenuItem {...props}>
                                        <Avatar src={option['avatarUrl']}></Avatar>
                                        <Box sx={{ pl: 1 }}>
                                            <Typography>{option['firstName']} {option['lastName']}</Typography>
                                            <Typography sx={{ color: 'text.disabled' }}>{option['email']}</Typography>
                                        </Box>
                                    </MenuItem>
                                }}
                                renderInput={(params) => (
                                    <TextField sx={{ mt: 2 }} variant="filled" fullWidth {...params}
                                        label={t('task.assignToUser')} required
                                        error={Boolean(errors?.assignedToUserId)} helperText={errors?.assignedToUserId?.message}
                                        InputLabelProps={{ shrink: true }} />
                                )}
                            />
                        )}>
                    </Controller> : <input type='hidden' {...register('assignedToUserId')} />}

                    <Controller control={control} name="status" render={({ field: {
                        onChange, value, ...rest
                    } }) => (
                        <FormControl variant="filled" sx={{ mt: 2 }} fullWidth>
                            <InputLabel id="demo-simple-select-filled-label" shrink required>
                                {t('common.status')}
                            </InputLabel>
                            <Select onChange={onChange} value={value} {...rest}
                                labelId="demo-simple-select-filled-label"
                                id="demo-simple-select-filled"
                                error={Boolean(errors?.status)}>

                                {allowedNewStatus.map(p => <MenuItem value={p}>
                                    {t(taskValidatorConfig.TaskStatusLang[p])}
                                </MenuItem>)}
                            </Select>
                            <FormHelperText error>{errors?.status?.message}</FormHelperText>
                        </FormControl>
                    )}></Controller>

                    <TextField multiline autoFocus margin="dense" label={t('common.description')}
                        fullWidth variant="filled" {...register('note')}
                        InputLabelProps={{ shrink: true }} sx={{ mt: 2 }}
                    />

                    <input type="hidden" value={project['id']} {...register('projectId')} />
                </DialogContent>
                <DialogActions>
                    <Button onClick={onCancel}>{t('common.cancel')}</Button>
                    <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
                </DialogActions>
            </Box>
        </Dialog>}

        {canShorten && <ConfirmDialog title={t('task.shorten.title')}
            renderOption={_ => {
                console.log(new Date(task['endDate']))
                const totalHours = (convertUTCDateToLocalDate(new Date(task['endDate'])) - new Date()) / (1000 * 60 * 60);
                return <Box>
                    <Chip color="success" label={<Remain totalHours={totalHours} showMinutes></Remain>}></Chip>
                    <Typography sx={{ color: 'text.secondary', mt: 1 }}>{t('task.shorten.message')}</Typography>
                </Box>
            }} onCancel={onShortenDialogCancel} onAgree={onShortenDialogAgree}></ConfirmDialog>}
    </>
}