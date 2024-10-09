import {
    Avatar,
    Box, Button, Card, CardActions, CardContent, Checkbox,
    Chip,
    FilledInput,
    FormControl, FormControlLabel, IconButton, InputLabel, ListItemIcon, ListItemText,
    Menu,
    MenuItem, Paper, Popover, Select, Skeleton, Stack,
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    TextField,
    Typography
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom"
import { APIContext, NotificationContext, UserContext } from "../../Common/Contexts";
import { endpoints } from "../../Config/API";
import AddIcon from '@mui/icons-material/Add';
import { convertHoursToRemainFormat, convertUTCDateToLocalDate, formatDate, formatDateTime } from "../../Utils/DayHelper";
import CircleIcon from '@mui/icons-material/Circle';
import { TaskStatusColors, TaskStatusLang } from "../Task/TaskValidatorConfiguration";
import RemoveRedEyeIcon from '@mui/icons-material/RemoveRedEye';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import CreateTaskForm from "../Task/CreateTaskForm";
import TaskOverviewDialog from "../Task/TaskOverviewDialog";
import FullscreenIcon from '@mui/icons-material/Fullscreen';
import UpdateTaskForm from "../Task/UpdateTaskForm";
import ConfirmDialog from "../Common/ConfirmDialog";
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import ZoomInMapIcon from '@mui/icons-material/ZoomInMap';
import { DateTimePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import SquareIcon from '@mui/icons-material/Square';
import Remain from "../Common/Remain";

//#region Member
const COL_WIDTH = 120;
const TIMELINE_Z_INDEX = 9999;
//#endregion

export default function ProjectTimeLine({ overview }) {

    //#region States
    const [filterAnchorEL, setFilterAnchorEL] = useState(null);
    const [timelineFilter, setTimelineFilter] = useState({});
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const [timelineDays, setTimelineDays] = useState([]);
    const [timelineTable, setTimelineTable] = useState([]);
    const [isShowCreateTaskForm, setShowCreateTaskForm] = useState(false);
    const [isLoadingTimeline, setLoadingTimeline] = useState(false);
    const [contextMenu, setContextMenu] = useState(null);
    const [overviewTaskId, setOverviewTaskId] = useState(null);
    const [updatedTaskId, setUpdatedTaskId] = useState(null);
    const [deleteTaskId, setdeleteTaskId] = useState(null);
    const { user, userDispatch } = useContext(UserContext);
    const { notificationManager } = useContext(NotificationContext);
    const nav = useNavigate();
    const [filter, setFilter] = useState([]);
    const [isZoomOut, setZoomOut] = useState(false);
    const [members, setMembers] = useState([]);
    const [isFilterApplied, setFilterApplied] = useState(false);
    //#endregion

    //#region Methods
    function onFilterClick(e) {
        setFilterAnchorEL(e.currentTarget);
    }

    function onFilterCloseClick() {
        setFilterAnchorEL(null);
    };

    function handleContextMenu(event, task) {
        event.preventDefault();
        setContextMenu(
            contextMenu === null
                ? {
                    mouseX: event.clientX + 2,
                    mouseY: event.clientY - 6,
                    task: task
                }
                : null,
        );
    };

    function handleContextMenuClose() {
        setContextMenu(null);
    };

    async function getTimeline(filter) {
        setLoadingTimeline(true);
        try {
            let url = endpoints['getTimeline'].replace('{0}', overview.id)
                .concat('?')
                .concat(Object.keys(filter).map(p => `${p}=${filter[p]}`).join('&'));

            const res = await bearerRequest.get(url);
            const data = res.data.result;
            setTimelineDays(data['days']);
            setTimelineTable(data['table']);
            setLoadingTimeline(false);
        } catch (error) {

        }
    }

    function onViewTaskOverviewClick(e) {
        handleContextMenuClose();
        setOverviewTaskId(contextMenu['task']['id']);
    }

    function onViewTaskOverviewClose(e) {
        setOverviewTaskId(null);
    }

    function onUpdateTaskFormClick() {
        handleContextMenuClose();
        setUpdatedTaskId(contextMenu['task']['id']);
    }

    function onUpdateTaskFormCancel() {
        setUpdatedTaskId(null);
    }

    function onUpdateTaskFormClose() {
        getTimeline(filter);
        setUpdatedTaskId(null);
    }

    function onDeleteTaskClick() {
        setdeleteTaskId(contextMenu['task']['id']);
        handleContextMenuClose();
    }

    async function deleteTask(id) {
        try {
            const res = await bearerRequest.delete(endpoints['deleteTask'].replace('{0}', id));
            notificationManager.showSuccess(t('task.delete.success'));
            getTimeline(filter);
            setdeleteTaskId(null);
        } catch (error) {

        }
    }

    function onCreateTaskFormCancel() {
        setShowCreateTaskForm(false);
    }

    function onCreateTaskFormSuccess() {
        getTimeline(filter);
        setShowCreateTaskForm(false);
    }

    function onStatusChange(event) {
        const {
            target: { value },
        } = event;
        setFilter(p => {
            return { ...p, status: typeof value === 'string' ? value.split(',') : value };
        });
    };

    function onAssignedToUserIdChange(event) {
        console.log(event.target.value)
        const {
            target: { value },
        } = event;
        setFilter(p => {
            return { ...p, assignedToUserIds: typeof value === 'string' ? value.split(',') : value };
        });
    };

    function onFilterApply() {
        setFilterApplied(true);
        getTimeline(filter);
        setFilterAnchorEL(null);
    }

    async function getMembers() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectMembers'].replace('{0}', overview['id']));
            setMembers(res['data']['result']);
        } catch (error) {

        }
    }

    function onKwChange(e) {
        setFilter(current => {
            return { ...current, kw: e.target.value };
        });
    }

    function onKwKeyDown(e) {
        if (e.repeat) { return; }
        if (e.key == 'Enter') {
            getTimeline(filter);
        }
    }

    function clearFilter() {
        setFilter(current => {
            return { kw: filter['kw'] ?? '' }
        });
        getTimeline({
            kw: filter['kw'] ?? ''
        });

        setFilterApplied(false);
    }

    function onIsLateCheckedChange(e) {
        setFilter(current => {
            return { ...current, isLate: e.target.checked };
        });
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getMembers();
        getTimeline(filter);
    }, [overview['beginDate'], overview['endDate']]);
    //#endregion


    return <>
        <Paper sx={{ p: 1, mt: 2 }} className={isZoomOut ? 'fullscreen' : ''}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography sx={{ mb: 1 }} variant="h6">{t('task.tasks')}</Typography>
                <IconButton onClick={_ => setZoomOut(p => !p)}>
                    {isZoomOut ? <ZoomInMapIcon></ZoomInMapIcon> : <FullscreenIcon></FullscreenIcon>}
                </IconButton>
            </Box>

            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <IconButton color="primary" onClick={() => setShowCreateTaskForm(true)}>
                    <AddIcon></AddIcon>
                </IconButton>
                <TextField variant="outlined" size="small" label={t('common.search')}
                    onChange={onKwChange} onKeyDown={onKwKeyDown}></TextField>

                <div>
                    <IconButton size="large" aria-controls="menu-appbar" aria-haspopup="true" onClick={onFilterClick} color="inherit">
                        <FilterAltIcon></FilterAltIcon>
                    </IconButton>
                    {isFilterApplied && <Chip label={t('common.filterApplied')} onDelete={clearFilter} />}
                    <Popover anchorEl={filterAnchorEL}
                        anchorOrigin={{
                            vertical: 'top',
                            horizontal: 'right',
                        }}
                        keepMounted
                        open={Boolean(filterAnchorEL)}
                        onClose={onFilterCloseClick}>
                        <Card sx={{ p: 2, minWidth: 400 }}>
                            <CardContent sx={{ p: 1 }}>
                                <Typography variant="subtitle1">{t('common.filter')}</Typography>
                                <Stack sx={{ mt: 2 }}>
                                    <FormControl variant="filled" size="small" fullWidth>
                                        <InputLabel id="tag-label">{t('common.status')}</InputLabel>
                                        <Select fullWidth
                                            labelId="tag-label"
                                            id="demo-multiple-checkbox"
                                            multiple
                                            value={filter['status'] ?? []}
                                            input={<FilledInput label="Tag" />}
                                            renderValue={(selected) => selected.map(p => t(TaskStatusLang[p])).join(', ')}
                                            onChange={onStatusChange}>
                                            {TaskStatusLang.map((name, index) => (
                                                <MenuItem key={index} value={index}>
                                                    <Checkbox checked={filter['status']?.indexOf(index) > -1} />
                                                    <ListItemText primary={t(name)} />
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>

                                    <FormControl variant="filled" sx={{ mt: 1 }} size="small">
                                        <InputLabel id="assigned-to-userid-label">{t('task.assignToUser')}</InputLabel>
                                        <Select
                                            labelId="assigned-to-userid-label"
                                            id="demo-multiple-checkbox"
                                            multiple
                                            value={filter['assignedToUserIds'] ?? []}
                                            input={<FilledInput label="Tag" />}
                                            renderValue={(selected) => selected.map(p => members.find(u => u['id'] == p)?.email).join(', ')}
                                            onChange={onAssignedToUserIdChange}>
                                            {members.map((member, index) => (
                                                <MenuItem key={index} value={member['id']}>
                                                    <Checkbox checked={filter.assignedToUserIds?.some(p => p == member['id'])} />
                                                    <Box sx={{ display: 'flex' }}>
                                                        <Avatar src={member['avatarUrl']}></Avatar>
                                                        <Box sx={{ pl: 1 }}>
                                                            <Typography>{member['firstName']} {member['lastName']}</Typography>
                                                            <Typography sx={{ color: 'text.disabled' }}>{member['email']}</Typography>
                                                        </Box>
                                                    </Box>
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>
                                    <FormControlLabel sx={{ mt: 2 }} control={<Checkbox />}
                                        label={t('task.late')} onChange={onIsLateCheckedChange} checked={filter?.isLate ?? false} />
                                </Stack>
                            </CardContent>
                            <CardActions sx={{ p: 1, display: "flex", justifyContent: 'space-between' }}>
                                <Button variant="outlined" onClick={onFilterApply}>{t('common.apply')}</Button>
                                <Button variant="outlined" onClick={onFilterCloseClick}>{t('common.close')}</Button>
                                <Button variant="outlined">{t('common.reset')}</Button>
                            </CardActions>
                        </Card>
                    </Popover>
                </div>
            </Box>
            <Box>
                {isLoadingTimeline
                    ? <Skeleton varian t="wave" sx={{ width: '100%', height: '200px', mt: 1 }} />
                    : <TableContainer component={Paper} sx={{ mt: 1, boxShadow: 'none' }}>
                        <Table sx={{ minWidth: 650, tableLayout: 'fixed' }} size="small">
                            <TableHead>
                                <TableRow>
                                    {timelineDays && timelineDays.map((p, i) => (
                                        <TableCell sx={{
                                            border: '1px solid', borderColor: 'text.disabled',
                                            borderBottom: 'none', borderTop: 'none', width: `${COL_WIDTH}px`, textAlign: 'center',
                                            backgroundColor: convertUTCDateToLocalDate(new Date(timelineDays[i])).valueOf() == new Date(new Date().toDateString()).valueOf() ? 'red' : 'inherit',
                                            // borderLeft: convertUTCDateToLocalDate(new Date(timelineDays[i])).valueOf() == new Date(new Date().toDateString()).valueOf() ? '1px red solid' : ''
                                        }}>
                                            <Typography variant="overline">
                                                {formatDate(convertUTCDateToLocalDate(new Date(p)))}
                                            </Typography>
                                        </TableCell>
                                    ))}
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {timelineTable && timelineTable.map((row, index) => (
                                    <TableRow key={index}>
                                        {row.map((cell, i) => (
                                            (cell?.isRendered == true)
                                                ? <TableCell id={cell['taskInfo']['id']} colSpan={parseInt(cell['colspan'])}
                                                    sx={{
                                                        p: 0, width: `${COL_WIDTH}px`, border: '1px solid',
                                                        borderColor: 'text.disabled'
                                                    }}
                                                    onContextMenu={(e) => handleContextMenu(e, cell['taskInfo'])}
                                                    onDoubleClick={() => setOverviewTaskId(cell['taskInfo']['id'])}>
                                                    <Box sx={{
                                                        backgroundColor: TaskStatusColors[parseInt(cell['taskInfo']['status'])],
                                                        cursor: 'pointer', ":hover": { border: '1px dashed white' }, p: 1,
                                                        border: '1px solid', borderColor: 'text.secondary'
                                                    }}>
                                                        <Typography fontSize='small' fontWeight='bold' sx={{ flexGrow: 1 }}>
                                                            {cell['taskInfo']['name']}
                                                        </Typography>
                                                        <Typography fontSize='small' sx={{ flexGrow: 1 }}>
                                                            {cell['taskInfo']['assignedToUser']['firstName']}&nbsp;
                                                            {cell['taskInfo']['assignedToUser']['lastName']}
                                                        </Typography>

                                                    </Box>
                                                </TableCell>
                                                : (cell == null ? <TableCell sx={{
                                                    border: '1px solid',
                                                    borderColor: 'text.disabled',
                                                }}></TableCell> : <></>)
                                        ))}
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>

                    </TableContainer>}

                <Box sx={{ display: 'flex', mt: 2 }}>
                    {TaskStatusLang.map((p, i) => <Box sx={{ display: 'flex', mr: 2, alignItems: 'center' }}>
                        <SquareIcon fontSize="small" sx={{ color: TaskStatusColors[i] }}></SquareIcon>
                        <Typography fontSize='small' color='text.secondary'>{t(p)}</Typography>
                    </Box>)}
                </Box>

                <Menu
                    open={contextMenu !== null}
                    onClose={handleContextMenuClose}
                    anchorReference="anchorPosition"
                    anchorPosition={
                        contextMenu !== null
                            ? { top: contextMenu.mouseY, left: contextMenu.mouseX }
                            : undefined
                    }>
                    <MenuItem onClick={(e) => onViewTaskOverviewClick(e)}>
                        <ListItemIcon>
                            <RemoveRedEyeIcon></RemoveRedEyeIcon>
                        </ListItemIcon>
                        <Box>
                            <Typography>{t('common.overview')}</Typography>
                            <Typography fontSize='small' color='text.disabled'>({t('common.doubleClick')})</Typography>
                        </Box>
                    </MenuItem>
                    <MenuItem onClick={(e) => nav(`/tasks/${contextMenu?.task.id}`)}>
                        <ListItemIcon>
                            <FullscreenIcon></FullscreenIcon>
                        </ListItemIcon>
                        <Box>
                            <Typography>{t('common.viewDetail')}</Typography>
                        </Box>
                    </MenuItem>
                    {(overview?.isManager || contextMenu?.task.assignedToUser.id == user?.id) && <MenuItem onClick={onUpdateTaskFormClick}>
                        <ListItemIcon>
                            <EditIcon></EditIcon>
                        </ListItemIcon>
                        <Typography>{t('common.update')}</Typography>
                    </MenuItem>}
                    {(overview?.isManager || contextMenu?.task.assignedToUser.id == user?.id) && <MenuItem onClick={onDeleteTaskClick}>
                        <ListItemIcon>
                            <DeleteIcon></DeleteIcon>
                        </ListItemIcon>
                        <Typography>{t('common.delete')}</Typography>
                    </MenuItem>}
                </Menu>
            </Box>
        </Paper>

        {isShowCreateTaskForm && <CreateTaskForm onCancel={onCreateTaskFormCancel}
            onSuccess={onCreateTaskFormSuccess}
            project={overview}></CreateTaskForm>}

        {overviewTaskId && <TaskOverviewDialog onClose={onViewTaskOverviewClose} taskId={overviewTaskId}></TaskOverviewDialog>}

        {updatedTaskId && <UpdateTaskForm onCancel={onUpdateTaskFormCancel} onSuccess={onUpdateTaskFormClose}
            taskId={updatedTaskId}></UpdateTaskForm>}

        {deleteTaskId && <ConfirmDialog onCancel={_ => setdeleteTaskId(null)}
            onAgree={_ => deleteTask(deleteTaskId)}
            title={t('task.delete.confirm')}
            message={t('task.delete.hint')}></ConfirmDialog>}
    </>
}