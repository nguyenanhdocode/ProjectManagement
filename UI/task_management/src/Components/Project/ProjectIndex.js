import { Accordion, AccordionDetails, AccordionSummary, Badge, Card, CardActions, CardContent, Chip, Divider, FormControl, IconButton, InputLabel, ListItemIcon, Menu, MenuItem, Popover, Select, Skeleton, Stack, Switch, TextField, Typography } from "@mui/material";
import { useContext, useEffect, useRef, useState } from "react"
import AccordionActions from '@mui/material/AccordionActions';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Button from '@mui/material/Button';
import { useTranslation } from "react-i18next";
import { styled } from '@mui/material/styles';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import Grid from '@mui/material/Grid';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { DateTimePicker, MobileDateTimePicker } from "@mui/x-date-pickers";
import CancelIcon from '@mui/icons-material/Cancel';
import AddIcon from '@mui/icons-material/Add';
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import API, { endpoints } from "../../Config/API";
import { convertToUTC, convertUTCDateToLocalDate, formatDateTime } from "../../Utils/DayHelper";
import { ProjectForm } from "./UpdateProjectForm";
import CreateProjectForm from "./CreateProjectForm";
import dayjs from "dayjs";
import ConfirmDialog from "../Common/ConfirmDialog";

const Item = styled(Card)(({ theme }) => ({
    backgroundColor: '#fff',
    variants: 'outlined',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    color: theme.palette.text.secondary,
    ...theme.applyStyles('dark', {
        backgroundColor: '#1A2027',
    }),
}));

export default function ProjectIndex() {

    //#region States
    let [projects, setProjects] = useState([]);
    const [t] = useTranslation();
    const kwRef = useRef();
    const [param, setParam] = useSearchParams();
    const { request, bearerRequest } = useContext(APIContext);
    const nav = useNavigate();
    const [showProjectForm, setShowProjectForm] = useState(false);
    const [filterAnchorEL, setFilterAnchorEL] = useState(null);
    const [deleteProjectId, setDeleteProjectId] = useState(null);
    const { notificationManager } = useContext(NotificationContext);
    //#endregion

    //#region Methods
    function onFilterClick(e) {
        setFilterAnchorEL(e.currentTarget);
    }

    function onFilterCloseClick() {
        setFilterAnchorEL(null);
    };

    function onKwKeyDown(e) {
        if (e.repeat) { return; }
        if (e.key == 'Enter') {
            setParam(current => {
                return { ...current, kw: kwRef.current?.value };
            });
        }
    }

    function onCancelProjectForm() {
        setShowProjectForm(false);
    }

    function onProjectFormSuccess() {
        setShowProjectForm(false);
        getAllProjects();
    }

    function clearFilter() {
        setParam({
            kw: kwRef.current.value,
        });
    }

    async function getAllProjects() {
        try {
            let url = `${endpoints['getAllProjects']}?`;

            const keys = Array.from(param.keys());
            for (let key of keys) {
                url += `${key}=${param.get(key)}&`;
            }

            const res = await bearerRequest.get(url);
            setProjects(res.data.result.projects);
        } catch (e) {

        }
    }

    function onBeginDateChange(value) {
        if (!value) return;
        setParam(current => {
            return { ...current, beginDate: new Date(value).toISOString() };
        });
    }

    function onEndDateChange(value) {
        setParam(current => {
            return { ...current, endDate: new Date(value).toISOString() };
        });
    }

    function isFilterApplied() {
        if (param.has('kw'))
            return param.size > 1;
        return param.size > 0;
    }

    function onJoinRoleChange(e) {
        setParam(current => {
            return { ...current, joinRole: e.target.value };
        });
    }

    async function deleteProject() {
        try {
            const res = await bearerRequest.delete(endpoints['deleteProject'].replace('{0}', deleteProjectId));
            notificationManager.showSuccess(t('project.delete.success'));
            getAllProjects();
            setDeleteProjectId(null);
        } catch (error) {
            
        }
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getAllProjects();
    }, [param]);
    //#endregion

    return <Box sx={{ p: 1 }}>
        <Typography variant="h6">{t('project.projectMgt')}</Typography>

        <Box sx={{ mt: 1, display: 'flex', alignItems: 'center' }}>
            <IconButton color="primary" onClick={() => setShowProjectForm(true)}>
                <AddIcon></AddIcon>
            </IconButton>
            <TextField sx={{ ml: 1 }} variant="outlined" label={t('common.search')} size="small"
                inputRef={kwRef} onKeyDown={onKwKeyDown}></TextField>

            <div>
                <IconButton size="large" aria-controls="menu-appbar" aria-haspopup="true" onClick={onFilterClick} color="inherit">
                    <FilterAltIcon></FilterAltIcon>
                </IconButton>
                {isFilterApplied() && <Chip label={t('common.filterApplied')} onDelete={clearFilter} />}
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
                                <FormControl fullWidth>
                                    <InputLabel size="small" id="demo-simple-select-label">
                                        {t('project.joinRole')}
                                    </InputLabel>
                                    <Select label={t('project.joinRole')} size="small" variant="filled"
                                        onChange={onJoinRoleChange} value={param.has('joinRole') ? param.get('joinRole') : 'all'}>
                                        <MenuItem value={'all'}>{t('common.all')}</MenuItem>
                                        <MenuItem value={'manager'}>{t('project.manager')}</MenuItem>
                                        <MenuItem value={'member'}>{t('project.member')}</MenuItem>
                                    </Select>
                                </FormControl>

                                <Typography sx={{ mt: 2 }} variant="subtitle2">{t('common.time')}</Typography>
                                <DateTimePicker sx={{ mt: 1 }} label={t('common.beginDate')}
                                    slotProps={{ textField: { size: 'small', variant: 'filled', }, field: { clearable: true } }}
                                    onChange={onBeginDateChange} value={param.has('beginDate') ? dayjs(param['beginDate']) : null}></DateTimePicker>

                                <DateTimePicker sx={{ mt: 1 }} label={t('common.endDate')}
                                    slotProps={{ textField: { size: 'small', variant: 'filled' } }}
                                    onChange={onEndDateChange} value={param.has('endDate') ? dayjs(param['endDate']) : null}></DateTimePicker>

                            </Stack>
                        </CardContent>
                        <CardActions sx={{ p: 1, display: "flex", justifyContent: 'space-between' }}>
                            <Button variant="outlined" onClick={onFilterCloseClick}>{t('common.close')}</Button>
                            <Button variant="outlined" onClick={clearFilter}>{t('common.reset')}</Button>
                        </CardActions>
                    </Card>
                </Popover>
            </div>
        </Box>
        {projects.length > 0 ? <Grid container spacing={2} sx={{ mt: 0 }}>
            {projects && projects.map(p => <Grid item xs={12} sm={6} md={4} lg={3}>
                <Item >
                    <CardContent sx={{ p: 0, height: 150 }}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                            <Typography gutterBottom sx={{ color: 'text.secondary', fontSize: 13 }}>
                                {p['id']}
                            </Typography>
                            <Chip size="small" color={p['isManager'] ? 'success' : 'warning'}
                                label={p['isManager'] ? t('project.manager') : t('project.member')}></Chip>
                        </Box>
                        <Typography sx={{ fontWeight: 'bold' }} variant="subtitle1" component="div">
                            {p['name']}
                        </Typography>
                        <Box sx={{ mt: 1 }}>
                            <Typography variant="body2">
                                {t('common.from')} <Chip size="small"
                                    label={formatDateTime(convertUTCDateToLocalDate(new Date(p['beginDate'])))}></Chip>
                            </Typography>
                            <Typography sx={{ mt: 1 }} variant="body2">
                                {t('common.to')} <Chip size="small"
                                    label={formatDateTime(convertUTCDateToLocalDate(new Date(p['endDate'])))}></Chip>
                            </Typography>
                        </Box>
                    </CardContent>
                    <CardActions sx={{ p: 0, pt: 1 }}>
                        <Button size="small" onClick={() => nav(`/projects/${p['id']}`)}>{t('common.detail')}</Button>
                        {p['isManager'] && <Button size="small" onClick={_ => setDeleteProjectId(p['id'])}>{t('common.delete')}</Button>}
                    </CardActions>
                </Item>
            </Grid>)}

        </Grid> : <Skeleton variant="wave" sx={{ width: '100%', height: '200px', mt: 1 }} />}

        {showProjectForm && <CreateProjectForm onCancel={onCancelProjectForm} onSuccess={onProjectFormSuccess}></CreateProjectForm>}

        {deleteProjectId && <ConfirmDialog title={t('project.delete.confirm')}
            message={t('project.delete.hint')} onCancel={_ => setDeleteProjectId(null)}
            onAgree={deleteProject}></ConfirmDialog>}
    </Box>
}