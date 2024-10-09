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

export default function CreateTaskForm({ onCancel, onSuccess, project }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [members, setMembers] = useState([]);
    const [assignedToUserId, setAssignedToUserId] = useState(null);
    const [tasks, setTasks] = useState(null);
    const [prevTask, setPrevTask] = useState(null);
    const {user, userDispatch} = useContext(UserContext);

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
                message: t('task.beginDate.mustLater')
                    .replace('{0}', formatDateTime(convertUTCDateToLocalDate(new Date(project['beginDate'])))),
                test: function (value) {
                    return value >= convertUTCDateToLocalDate(new Date(project['beginDate']));
                }
            })
            .test({
                name: 'mustLaterPrevTask',
                exclusive: false,
                message: t('task.beginDate.mustLater')
                    .replace('{0}', formatDateTime(convertUTCDateToLocalDate(new Date(prevTask?.endDate)))),
                test: function (value) {
                    if (watch('previousTaskId')) {
                        const task = tasks.find(p => p['id'] == watch('previousTaskId'));
                        return value >= convertUTCDateToLocalDate(new Date(task['endDate']))
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
                message: t('task.endDate.mustBefore')
                    .replace('{0}', formatDateTime(convertUTCDateToLocalDate(new Date(project['endDate'])))),
                test: function (value) {
                    return value <= convertUTCDateToLocalDate(new Date(project['endDate']))
                }
            }),
        assignedToUserId: yup.string().required(t('task.assignToUser.required')),
        projectId: yup.string().required(),
        previousTaskId: yup.string(),
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
            const res = await bearerRequest.post(endpoints['createTask'], data);
            notificationManager.showSuccess(t('task.created.success'));
            setProcessing(false);
            onSuccess();
        } catch (error) {
            setProcessing(false);
        }
    }

    async function getMembers() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectMembers'].replace('{0}', project['id']));
            setMembers(res['data']['result']);
        } catch (error) {

        }
    }

    function onAssignToUserChange(e, value) {
        console.log(value)
        setAssignedToUserId(value?.id);
    }

    async function getTasks() {
        try {
            const res = await bearerRequest.get(endpoints['getAvaiablePrevTask'].replace('{0}', project['id']));
            setTasks(res['data']['result']);
        } catch (error) {

        }
    }

    function onPrevTaskChange(e, value) {
        setPrevTask(value);
        setValue('beginDate', dayjs(convertUTCDateToLocalDate(new Date(value?.endDate || project.beginDate))));
    }
    //#endregion

    //#region Hooks
    useEffect(() => {

        setValue('assignedToUserId', user['id']);

        getMembers();
        getTasks();
    }, []);
    //#endregion

    return <>
        <Dialog open={true}
            maxWidth='sm' fullWidth={true} sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('task.create')}</DialogTitle>
            <Box component='form' onSubmit={handleSubmit(onSubmit)} noValidate>
                <DialogContent sx={{ pt: 0 }}>
                    <TextField {...register('name')} variant='filled' label={t('common.name')}
                        fullWidth error={Boolean(errors?.name)} helperText={errors?.name?.message}
                        required></TextField>

                    <Controller control={control} name="previousTaskId"
                        render={({ field: { onChange, value } }) => (
                            <Autocomplete
                                options={tasks}
                                getOptionLabel={p => {
                                    var task = tasks.find((v) => v['id'] === p);
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
                                        label={t('task.previousTask')} />
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
                                    ></CustomDateTimePicker>
                                </Box>
                            </Grid>
                        </Grid>
                    </Box>

                    {project['isManager'] ? <Controller control={control} name="assignedToUserId"
                        render={({ field: { onChange, value } }) => (
                            <Autocomplete
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
                                        error={Boolean(errors?.assignedToUserId)} helperText={errors?.assignedToUserId?.message} />
                                )}
                            />
                        )}>
                    </Controller> : <input type="hidden" {...register('assignedToUserId')} />}

                    <TextField multiline autoFocus margin="dense" label={t('common.description')}
                        fullWidth variant="filled" {...register('note')}
                    />

                    <input type="hidden" value={project['id']} {...register('projectId')} />
                </DialogContent>
                <DialogActions>
                    <Button onClick={onCancel}>{t('common.cancel')}</Button>
                    <LoadingButton type='submit' loading={isProcessing}>{t('common.save')}</LoadingButton>
                </DialogActions>
            </Box>
        </Dialog>
    </>
}