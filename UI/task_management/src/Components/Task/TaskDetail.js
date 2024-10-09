import { useContext, useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom"
import { APIContext, NotificationContext, UserContext } from "../../Common/Contexts";
import { endpoints } from "../../Config/API";
import { Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, IconButton, Paper, Skeleton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography } from "@mui/material";
import TaskOverview from "./TaskOverview";
import AddIcon from '@mui/icons-material/Add';
import CreateSubtaskForm from "./CreateSubtaskForm";
import { convertUTCDateToLocalDate, formatDateTime } from "../../Utils/DayHelper";
import { TaskStatusColors, TaskStatusLang } from "./TaskValidatorConfiguration";
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import UpdateSubtaskForm from "./UpdateSubtaskForm";

export default function TaskDetail() {

    //#region States
    const { id } = useParams();
    const [task, setTask] = useState(null);
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [subtasks, setSubtasks] = useState([]);
    const [isShowCreateSubtaskForm, setShowCreateSubtaskForm] = useState(false);
    const { user, _ } = useContext(UserContext);
    const kwRef = useRef();
    const [updatedSubtaskId, setUpdatedSubtaskId] = useState(null);
    const [deleteTaskId, setDeleteTaskId] = useState(null);

    //#endregion

    //#region Methods
    async function getTask(id) {
        try {
            const res = await bearerRequest.get(endpoints['getTask'].replace('{0}', id));
            const data = res.data.result;
            setTask(data);
        } catch (error) {

        }
    }

    async function getSubtasks(id) {
        try {
            let url = endpoints['getSubtasks'].replace('{0}', id);
            url = `${url}?kw=${kwRef.current?.value || ''}`
            const res = await bearerRequest.get(url);
            const data = res.data.result;
            setSubtasks(data);
        } catch (error) {

        }
    }

    function onCreateSubtaskFormSuccess() {
        getSubtasks(id);
        setShowCreateSubtaskForm(false);
    }

    function onKwKeyDown(e) {
        if (e.repeat) { return; }
        if (e.key == 'Enter') {
            getSubtasks(id);
        }
    }

    function onUpdateSubtaskFormSuccess() {
        getSubtasks(id);
        setUpdatedSubtaskId(null);
    }

    async function deleteTask(deleteId) {
        try {
            const res = await bearerRequest.delete(endpoints['deleteTask'].replace('{0}', deleteId));
            notificationManager.showSuccess(t('task.delete.success'));
            getSubtasks(id);
            setDeleteTaskId(null);
        } catch (error) {

        }
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getTask(id);
        getSubtasks(id);
    }, []);
    //#endregion

    return <>
        <Box sx={{ p: 1 }}>
            {task && <Typography variant="h6">{task['name']}</Typography>}
            <Paper sx={{ mt: 1, p: 1 }}>
                {task && <Typography variant="h6" sx={{ mb: 1 }}>{t('common.overview')}</Typography>}
                {task ? <TaskOverview taskId={id}></TaskOverview> : <Skeleton variant="wave" sx={{ width: '100%', height: '200px', mt: 1 }} />}
            </Paper>

            <Paper sx={{ mt: 1, p: 1 }}>
                {task && <Typography variant="h6" sx={{ mb: 1 }}>{t('task.subtasks')}</Typography>}
                {task && <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    {(task['assignedToUser']['id'] == user['id'] || task['createdUser']['id'] == user['id'])
                        && <IconButton color="primary" onClick={() => setShowCreateSubtaskForm(true)}>
                            <AddIcon></AddIcon>
                        </IconButton>}
                    <TextField variant="outlined" size="small" label={t('common.search')}
                        onKeyDown={onKwKeyDown} inputRef={kwRef}></TextField>
                </Box>}
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>{t('common.id')}</TableCell>
                                <TableCell>{t('common.name')}</TableCell>
                                <TableCell>{t('common.beginDate')}</TableCell>
                                <TableCell>{t('common.endDate')}</TableCell>
                                <TableCell>{t('common.status')}</TableCell>
                                <TableCell>{t('common.note')}</TableCell>
                                <TableCell>
                                </TableCell>
                                <TableCell></TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {subtasks.length > 0 && subtasks.map(p => (
                                <TableRow>
                                    <TableCell>{p['id']}</TableCell>
                                    <TableCell>{p['name']}</TableCell>
                                    <TableCell>{formatDateTime(convertUTCDateToLocalDate(new Date(p['beginDate'])))}</TableCell>
                                    <TableCell>{formatDateTime(convertUTCDateToLocalDate(new Date(p['endDate'])))}</TableCell>
                                    <TableCell>
                                        <Chip sx={{ backgroundColor: TaskStatusColors[parseInt(p['status'])] }} label={t(TaskStatusLang[parseInt(p['status'])])}></Chip>
                                    </TableCell>
                                    <TableCell>
                                        {p['note']}
                                    </TableCell>
                                    <TableCell>
                                        {(task['createdUser']['id'] == user['id']
                                            || task['assignedToUser']['id'] == user['id'])
                                            && <IconButton sx={{ ml: 1 }} size="small" onClick={() => setUpdatedSubtaskId(p['id'])}>
                                                <EditIcon></EditIcon>
                                            </IconButton>}
                                        {(task['createdUser']['id'] == user['id']
                                            || task['assignedToUser']['id'] == user['id'])
                                            && <IconButton sx={{ ml: 1 }} size="small" onClick={() => setDeleteTaskId(p['id'])}>
                                                <DeleteIcon></DeleteIcon>
                                            </IconButton>}
                                    </TableCell>
                                    <TableCell></TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                    {subtasks.length == 0 && <Typography textAlign='center' sx={{ mt: 1 }}>
                        {t('common.noData')}
                    </Typography>}
                </TableContainer>
            </Paper>
        </Box>

        {isShowCreateSubtaskForm && <CreateSubtaskForm taskId={task['id']} onCancel={() => setShowCreateSubtaskForm(false)}
            onSuccess={onCreateSubtaskFormSuccess}></CreateSubtaskForm>}

        {updatedSubtaskId && <UpdateSubtaskForm taskId={updatedSubtaskId} parentId={task['id']} onSuccess={onUpdateSubtaskFormSuccess}
            onCancel={() => setUpdatedSubtaskId(null)}></UpdateSubtaskForm>}

        <Dialog
            open={deleteTaskId != null}
            onClose={() => setDeleteTaskId(null)}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description">
            <DialogTitle id="alert-dialog-title">
                {t('task.delete.confirm')}
            </DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    {t('task.delete.hint')}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setDeleteTaskId(null)}>
                    {t('common.cancel')}
                </Button>
                <Button onClick={() => deleteTask(deleteTaskId)} autoFocus>
                    {t('common.delete')}
                </Button>
            </DialogActions>
        </Dialog>
    </>
}