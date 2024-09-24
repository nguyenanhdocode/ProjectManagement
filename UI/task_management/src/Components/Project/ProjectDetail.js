import { Accordion, AccordionDetails, AccordionSummary, Avatar, Badge, Box, Card, CardContent, CardHeader, Chip, CircularProgress, Grid, IconButton, List, ListItemButton, ListItemIcon, ListItemText, Menu, MenuItem, Paper, Skeleton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom"
import { APIContext } from "../../Common/Contexts";
import { endpoints } from "../../Config/API";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import AddIcon from '@mui/icons-material/Add';
import { convertUTCDateToLocalDate, formatDate, formatDateTime } from "../../Utils/DayHelper";
import TaskForm from "../Task/TaskForm";
import CircleIcon from '@mui/icons-material/Circle';
import { TaskStatusColors } from "../Task/TaskValidatorConfiguration";
import RemoveRedEyeIcon from '@mui/icons-material/RemoveRedEye';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { ProjectForm, UpdateProjectForm } from "./UpdateProjectForm";
import UpdateProjectBeginDateForm from "./UpdateProjectBeginDateForm";

export default function ProjectDetail() {

    //#region Member
    const COL_WIDTH = 120;
    const COL_MARGIN = 5;
    //#endregion

    //#region States
    const [overview, setOverview] = useState();
    const { id } = useParams();
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const [timelineDays, setTimelineDays] = useState([]);
    const [timelineTable, setTimelineTable] = useState([]);
    const [isShowTaskForm, setShowTaskForm] = useState(false);
    const [isLoadingTimeline, setLoadingTimeline] = useState(false);
    const [contextMenu, setContextMenu] = useState(null);
    const [isShowProjectForm, setShowProjectForm] = useState(false);
    const [isShowUpdateBeginDateForm, setShowUpdateBeginDateForm] = useState(false);

    //#endregion

    //#region Methods
    async function getOverview() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectOverview'].replace('{0}', id));
            const data = res.data.result;
            setOverview(data);
        } catch (error) {

        }
    }

    async function getTimeline() {
        setLoadingTimeline(true);
        try {
            const res = await bearerRequest.get(endpoints['getTimeline'].replace('{0}', id));
            const data = res.data.result;
            setTimelineDays(data['days']);
            setTimelineTable(data['table']);
            setLoadingTimeline(false);
        } catch (error) {

        }
    }

    function onTaskFormCancel() {
        setShowTaskForm(false);
    }

    function onTaskFormSuccess() {
        getTimeline();
        setShowTaskForm(false);
    }


    function handleContextMenu(event) {
        event.preventDefault();
        setContextMenu(
            contextMenu === null
                ? {
                    mouseX: event.clientX + 2,
                    mouseY: event.clientY - 6,
                }
                : null,
        );
    };

    function handleClose() {
        setContextMenu(null);
    };

    function onProjectFormCancel() {
        setShowProjectForm(false);
    }

    function onProjectFormSuccess() {
        getOverview();
        setShowProjectForm(false);
    }
    
    function onUpdateBeginDateFormCancel() {
        setShowUpdateBeginDateForm(false);
    }

    function onUPdateBeginDateFormSuccess() {
        setShowUpdateBeginDateForm(false);
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getOverview();
        getTimeline();
    }, []);
    //#endregion

    return <>
        <Box sx={{ p: 1 }}>
            {overview && (<>
                <Typography variant="h6">{overview['name']}</Typography>

                <Paper sx={{ p: 1, mt: 1 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Typography variant="subtitle1">{t('project.overview')}</Typography>
                        <IconButton onClick={() => setShowProjectForm(true)}>
                            <EditIcon></EditIcon>
                        </IconButton>
                    </Box>
                    <TableContainer component={Paper} sx={{ boxShadow: 'none' }}>
                        <Table sx={{ minWidth: 650 }} aria-label="simple table">
                            <TableRow>
                                <TableCell>{t('common.id')}</TableCell>
                                <TableCell>{overview['id']}</TableCell>
                                <TableCell>{t('common.name')}</TableCell>
                                <TableCell>{overview['name']}</TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>{t('common.beginDate')}</TableCell>
                                <TableCell>
                                    {formatDateTime(convertUTCDateToLocalDate(new Date(overview['beginDate'])))}
                                    <IconButton sx={{ ml: 1 }} size="small" onClick={() => setShowUpdateBeginDateForm(true)}>
                                        <EditIcon></EditIcon>
                                    </IconButton>
                                </TableCell>
                                <TableCell>{t('common.endDate')}</TableCell>
                                <TableCell>
                                    {formatDateTime(convertUTCDateToLocalDate(new Date(overview['endDate'])))}
                                    <IconButton sx={{ ml: 1 }} size="small" onClick={() => setShowProjectForm(true)}>
                                        <EditIcon></EditIcon>
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>{t('common.description')}</TableCell>
                                <TableCell colspan={4}>{overview['description']}</TableCell>
                            </TableRow>
                        </Table>
                    </TableContainer>
                </Paper>

                <Paper sx={{ p: 1, mt: 2 }}>
                    <Typography sx={{ mb: 1 }} variant="subtitle1">{t('task.tasks')}</Typography>

                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <IconButton color="primary" onClick={() => setShowTaskForm(true)}>
                            <AddIcon></AddIcon>
                        </IconButton>
                        <TextField variant="outlined" size="small" label={t('common.search')}></TextField>
                    </Box>
                    <Box>

                        {isLoadingTimeline
                            ? <Skeleton variant="wave" sx={{ width: '100%', height: '200px', mt: 1 }} />
                            : <TableContainer component={Paper} sx={{ mt: 1, boxShadow: 'none' }}>
                                <Table sx={{ minWidth: 650, tableLayout: 'fixed' }}>
                                    <TableHead>
                                        <TableRow>
                                            {timelineDays && timelineDays.map((p, i) => (
                                                <TableCell sx={{
                                                    border: '1px solid', borderColor: 'text.disabled',
                                                    borderBottom: 'none', borderTop: 'none',
                                                    borderRight: i + 1 > timelineDays.length ? 'none' : 'inherit',
                                                    pl: 0, width: `${COL_WIDTH}px`
                                                }}>
                                                    <Typography variant="overline">{formatDate(convertUTCDateToLocalDate(new Date(p)))}</Typography>
                                                </TableCell>
                                            ))}
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {timelineTable && timelineTable.map(row => (
                                            <TableRow>
                                                {row.map((cell, i) => (
                                                    (cell && cell['isRendered'])
                                                        ? <TableCell id={cell['taskInfo']['id']} colSpan={parseInt(cell['colspan'])}
                                                            sx={{
                                                                p: 0, width: `${COL_WIDTH}px`, border: '1px solid',
                                                                borderColor: 'text.disabled'
                                                            }} onContextMenu={handleContextMenu}>
                                                            <Card sx={{
                                                                backgroundColor: TaskStatusColors[parseInt(cell['taskInfo']['status'])],
                                                                cursor: 'pointer', ":hover": { border: '1px dashed white' },
                                                            }}>
                                                                <CardContent sx={{ p: 1, borderLeft: '3px solid red' }}>
                                                                    <Typography variant="body2" sx={{ flexGrow: 1 }}>{cell['taskInfo']['name']}</Typography>
                                                                </CardContent>
                                                            </Card>
                                                        </TableCell>
                                                        : <TableCell sx={{ border: '1px solid', borderColor: 'text.disabled' }}></TableCell>
                                                ))}
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            </TableContainer>}

                        <Menu
                            open={contextMenu !== null}
                            onClose={handleClose}
                            anchorReference="anchorPosition"
                            anchorPosition={
                                contextMenu !== null
                                    ? { top: contextMenu.mouseY, left: contextMenu.mouseX }
                                    : undefined
                            }
                        >
                            <MenuItem onClick={handleClose}>
                                <ListItemIcon>
                                    <RemoveRedEyeIcon></RemoveRedEyeIcon>
                                </ListItemIcon>
                                <Box>
                                    <Typography>{t('common.viewDetail')}</Typography>
                                    <Typography fontSize='small' color='text.disabled'>or double click</Typography>
                                </Box>
                            </MenuItem>
                            <MenuItem onClick={handleClose}>
                                <ListItemIcon>
                                    <EditIcon></EditIcon>
                                </ListItemIcon>
                                <Typography>{t('common.update')}</Typography>
                            </MenuItem>
                            <MenuItem onClick={handleClose}>
                                <ListItemIcon>
                                    <DeleteIcon></DeleteIcon>
                                </ListItemIcon>
                                <Typography>{t('common.delete')}</Typography>
                            </MenuItem>
                        </Menu>
                    </Box>
                </Paper>

                <Paper sx={{ p: 1, mt: 2 }}>
                    <Typography sx={{ mb: 1 }} variant="subtitle1">{t('asset.asset')}</Typography>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <IconButton color="primary">
                            <AddIcon></AddIcon>
                        </IconButton>
                        <TextField variant="outlined" size="small" label={t('common.search')}></TextField>
                    </Box>
                </Paper>
            </>)}

            {isShowTaskForm && <TaskForm onCancel={onTaskFormCancel}
                onSuccess={onTaskFormSuccess}
                project={overview}></TaskForm>}

            {isShowProjectForm && <UpdateProjectForm onCancel={onProjectFormCancel}
                onSuccess={onProjectFormSuccess} projectId={overview['id']}>
            </UpdateProjectForm>}

            {isShowUpdateBeginDateForm && <UpdateProjectBeginDateForm onCancel={onUpdateBeginDateFormCancel} onSuccess={onProjectFormCancel}
                projectId={overview['id']}>
            </UpdateProjectBeginDateForm>}
        </Box>
    </>
}