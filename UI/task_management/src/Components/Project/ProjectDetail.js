import { Accordion, AccordionDetails, AccordionSummary, Avatar, Badge, Box, Button, Card, CardContent, CardHeader, Checkbox, Chip, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FilledInput, FormControl, Grid, IconButton, InputLabel, Link, List, ListItemButton, ListItemIcon, ListItemText, Menu, MenuItem, Paper, Popover, Select, Skeleton, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom"
import { APIContext, NotificationContext, UserContext } from "../../Common/Contexts";
import { endpoints } from "../../Config/API";
import AddIcon from '@mui/icons-material/Add';
import { convertUTCDateToLocalDate, formatDate, formatDateTime } from "../../Utils/DayHelper";
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { UpdateProjectForm } from "./UpdateProjectForm";
import UpdateProjectBeginDateForm from "./UpdateProjectBeginDateForm";
import UpdateProjectEndDateForm from "./UpdateProjectEndDateForm";
import FindUserDialog from "../User/FindUserDialog";
import ChooseAssetsDialog from "../Asset/ChooseAssetsDialog";
import { ImageExtensions } from "../Asset/AssetValidatorConfiguration";
import ConfirmDialog from "../Common/ConfirmDialog";
import ProjectTimeLine from "./ProjectTimeLine";
import Remain from "../Common/Remain";

export default function ProjectDetail() {

    //#region States
    const [overview, setOverview] = useState(null);
    const { id } = useParams();
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const [isShowUpdateProjectForm, setShowUpdateProjectForm] = useState(false);
    const [isShowUpdateBeginDateForm, setShowUpdateBeginDateForm] = useState(false);
    const [isShowUpdateEndDateForm, setShowUpdateEndDateForm] = useState(false);
    const { user, userDispatch } = useContext(UserContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isShowFindUserDialog, setShowFindUserDialog] = useState(false);
    const [members, setMembers] = useState([]);
    const [isShowChooseAssetsDialog, setShowChooseAssetsDialog] = useState(false);
    const [assets, setAssets] = useState([]);
    const [deleteAssetId, setDeleteAssetId] = useState(null);
    const [removeMemberId, setRemoveMemberId] = useState();
    const nav = useNavigate();
    const [progress, setProgress] = useState(0);
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

    async function getMembers() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectMembers'].replace('{0}', id));
            const data = res.data.result;
            setMembers(data);
        } catch (error) {

        }
    }

    function onProjectFormCancel() {
        setShowUpdateProjectForm(false);
    }

    function onProjectFormSuccess() {
        getOverview();
        setShowUpdateProjectForm(false);
    }

    function onUpdateBeginDateFormCancel() {
        setShowUpdateBeginDateForm(false);
    }

    function onUpdateBeginDateFormSuccess() {
        getOverview();
        setShowUpdateBeginDateForm(false);
    }

    function onUpdateEndDateFormCancel() {
        setShowUpdateEndDateForm(false);
    }

    function onUpdateEndDateFormSuccess() {
        getOverview();
        setShowUpdateEndDateForm(false);
    }

    async function onFindUserDialogDone(selectedUsers) {
        const listId = selectedUsers.map(p => p['id']);

        try {
            const res = await bearerRequest
                .post(endpoints['addProjectMembers'].replace('{0}', overview['id']), {
                    userIds: listId
                });
            notificationManager.showSuccess(t('project.addMember.success'));
            getMembers();
        } catch (error) {

        }
        setShowFindUserDialog(false);
    }

    async function removeMember() {
        try {
            const url = endpoints['removeMember'].replace('{0}', overview['id'])
                .replace('{1}', removeMemberId);
            const res = await bearerRequest.delete(url);
            notificationManager.showSuccess(t('project.removeMember.success'));
            getMembers();
        } catch (error) {

        }
        setRemoveMemberId(null);
    }

    function onChooseAssetsDialogClose() {
        setShowChooseAssetsDialog(false);
    }

    async function onChooseAssetsDialogDone(assets) {
        try {
            const res = await bearerRequest
                .post(endpoints['addProjectAssets'].replace('{0}', overview['id']), {
                    assetIds: assets.map(p => p['id'])
                });
            notificationManager.showSuccess(t('project.addAssets.success'));
            getAssets();
        } catch (error) {

        }
        setShowChooseAssetsDialog(false);
    }

    async function getAssets() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectAssets'].replace('{0}', id));
            const data = res.data.result;
            setAssets(data);
        } catch (error) {

        }
    }

    async function removeAsset() {
        try {
            const url = endpoints['removeAssetFromProject'].replace('{0}', overview['id'])
                .replace('{1}', deleteAssetId);
            const res = await bearerRequest.delete(url);
            notificationManager.showSuccess(t('project.removeAsset.success'));
            getAssets();
        } catch (error) {

        }
        setDeleteAssetId(null);
    }

    async function getProgress() {
        try {
            const res = await bearerRequest.get(endpoints['getProjectProgress'].replace('{0}', id));
            const data = res.data.result;
            setProgress(parseFloat(data).toFixed(1));
        } catch (error) {

        }
    }

    //#endregion

    //#region Hooks
    useEffect(() => {
        getOverview();
        getProgress();
        getMembers();
        getAssets();
    }, []);
    //#endregion

    return <>
        <Box sx={{ p: 1 }}>

            {overview && <Typography variant="h6">{overview['name']}</Typography>}

            {overview ? <Paper sx={{ p: 1, mt: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="h6">{t('project.overview')}</Typography>
                    {overview['isManager'] && <IconButton onClick={() => setShowUpdateProjectForm(true)}>
                        <EditIcon></EditIcon>
                    </IconButton>}
                </Box>
                {overview && <TableContainer component={Paper} sx={{ boxShadow: 'none' }}>
                    <Table sx={{ minWidth: 650 }} aria-label="simple table" size="small">
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
                                {overview['isManager'] && <IconButton sx={{ ml: 1 }} size="small" onClick={() => setShowUpdateBeginDateForm(true)}>
                                    <EditIcon fontSize="small"></EditIcon>
                                </IconButton>}
                            </TableCell>
                            <TableCell>{t('common.endDate')}</TableCell>
                            <TableCell>
                                {formatDateTime(convertUTCDateToLocalDate(new Date(overview['endDate'])))}
                                {overview['isManager'] && <IconButton sx={{ ml: 1 }} size="small" onClick={() => setShowUpdateEndDateForm(true)}>
                                    <EditIcon fontSize="small"></EditIcon>
                                </IconButton>}
                                <Chip sx={{ ml: 1 }} size="small" color="success" label={
                                    <Remain totalHours={parseInt(overview['remainHours'])} sx={{ fontSize: 'small' }}>
                                    </Remain>
                                }>
                                </Chip>
                            </TableCell>
                        </TableRow>
                        <TableRow>
                            <TableCell>{t('common.progress')}</TableCell>
                            <TableCell>
                                <Box sx={{ position: 'relative', display: 'inline-flex' }}>
                                    <CircularProgress color="success" variant="determinate" value={progress} />
                                    <Box
                                        sx={{
                                            top: 0,
                                            left: 0,
                                            bottom: 0,
                                            right: 0,
                                            position: 'absolute',
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: 'center',
                                        }}
                                    >
                                        <Typography
                                            variant="caption"
                                            component="div"
                                            sx={{ color: 'text.secondary' }}
                                        >
                                            {`${Math.round(progress)}%`}
                                        </Typography>
                                    </Box>
                                </Box>
                            </TableCell>
                            <TableCell>{t('common.description')}</TableCell>
                            <TableCell>{overview['description']}</TableCell>
                        </TableRow>
                    </Table>
                </TableContainer>}
            </Paper> : <Skeleton variant="wave" sx={{ width: '100%', height: '200px', mt: 1 }} />}

            {overview && <ProjectTimeLine overview={overview}></ProjectTimeLine>}

            <Paper sx={{ p: 1, mt: 2 }}>
                <Typography sx={{ mb: 1 }} variant="h6">{t('project.member')}</Typography>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    {overview?.isManager && <IconButton color="primary" onClick={() => setShowFindUserDialog(true)}>
                        <AddIcon></AddIcon>
                    </IconButton>}  
                    {members.map(p => <Chip sx={{ mr: 1 }} label={p['email']}
                        onDelete={
                            (overview?.isManager && p['id'] != user['id'])
                                ? () => setRemoveMemberId(p['id'])
                                : null
                        }></Chip>)}
                </Box>
            </Paper>

            <Paper sx={{ p: 1, mt: 2 }}>
                <Typography sx={{ mb: 1 }} variant="h6">{t('asset.asset')}</Typography>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <IconButton color="primary" onClick={() => setShowChooseAssetsDialog(true)}>
                        <AddIcon></AddIcon>
                    </IconButton>
                </Box>

                <TableContainer>
                    <Table size="small">
                        <TableHead>
                            <TableCell>{t('common.name')}</TableCell>
                            <TableCell>{t('asset.format')}</TableCell>
                            <TableCell>{t('asset.size')}</TableCell>
                            <TableCell>{t('common.createdUser')}</TableCell>
                            <TableCell></TableCell>
                        </TableHead>
                        <TableBody>
                            {assets && assets.map(p => (<TableRow>
                                <TableCell>
                                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                        <img width={24} height={24}
                                            src={ImageExtensions.includes(p['type']) ? p['url'] : `../IconsPack/${p['type']}.svg`} />

                                        <Link target="blank" href={p['url']}>
                                            <Typography sx={{ ml: 1 }} fontSize='small'>{p['displayFileName']}</Typography>
                                        </Link>
                                    </Box>
                                </TableCell>
                                <TableCell>{p['type']}</TableCell>
                                <TableCell>{(parseFloat(p['size']) / 1024).toFixed(2)} KB</TableCell>
                                <TableCell>{p?.createdUser?.email}</TableCell>
                                <TableCell>
                                    <IconButton size="small" onClick={_ => setDeleteAssetId(p['id'])}>
                                        <DeleteIcon></DeleteIcon>
                                    </IconButton>
                                </TableCell>
                            </TableRow>))}
                        </TableBody>
                    </Table>
                </TableContainer>
                {assets.length == 0 && <Typography textAlign='center' sx={{ mt: 1 }}>
                    {t('asset.noAssets')}
                </Typography>}
            </Paper>

            {isShowUpdateProjectForm && <UpdateProjectForm onCancel={onProjectFormCancel}
                onSuccess={onProjectFormSuccess} projectId={overview['id']}>
            </UpdateProjectForm>}

            {isShowUpdateBeginDateForm && <UpdateProjectBeginDateForm onCancel={onUpdateBeginDateFormCancel} onSuccess={onUpdateBeginDateFormSuccess}
                projectId={overview['id']}>
            </UpdateProjectBeginDateForm>}

            {isShowUpdateEndDateForm && <UpdateProjectEndDateForm onCancel={onUpdateEndDateFormCancel} onSuccess={onUpdateEndDateFormSuccess}
                projectId={overview['id']}>
            </UpdateProjectEndDateForm>}

            {isShowFindUserDialog && <FindUserDialog onCancel={() => setShowFindUserDialog(false)}
                onDone={onFindUserDialogDone}></FindUserDialog>}

            {isShowChooseAssetsDialog && <ChooseAssetsDialog onCancel={onChooseAssetsDialogClose}
                onDone={onChooseAssetsDialogDone}></ChooseAssetsDialog>}

            {deleteAssetId && <ConfirmDialog title={t('project.removeAsset.confirm')}
                message={t('project.removeAsset.hint')}
                onCancel={_ => setDeleteAssetId(null)}
                onAgree={_ => removeAsset()}>
            </ConfirmDialog>}

            {removeMemberId && <ConfirmDialog title={t('project.removeMember.confirm')}
                message={t('project.removeMember.hint')}
                onCancel={_ => setRemoveMemberId(null)}
                onAgree={_ => removeMember()}>
            </ConfirmDialog>}
        </Box >
    </>
}