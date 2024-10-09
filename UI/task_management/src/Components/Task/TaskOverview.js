import { Avatar, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle, Grid, Link, Paper, Table, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import { endpoints } from "../../Config/API";
import { convertUTCDateToLocalDate, formatDateTime } from "../../Utils/DayHelper";
import dayjs from "dayjs";
import { TaskStatusColors, TaskStatusLang } from "./TaskValidatorConfiguration";
import Remain from "../Common/Remain";

export default function TaskOverview({ onClose, taskId }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [overview, setOverview] = useState(null);
    //#endregion

    //#region Methods
    async function getOverview() {
        try {
            const res = await bearerRequest.get(endpoints['getTaskOverview'].replace('{0}', taskId));
            const data = res.data.result;
            setOverview(data);
            console.log(data)
        } catch (error) {

        }
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getOverview();
    }, []);
    //#endregion

    return <>
        {overview && <Grid container spacing={2}>
            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.id')}</Typography>
                    <Typography sx={{ width: '50%' }}>{overview['id']}</Typography>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.name')}</Typography>
                    <Typography sx={{ width: '50%' }}>{overview['name']}</Typography>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.beginDate')}</Typography>
                    <Typography sx={{ width: '50%' }}>
                        {formatDateTime(convertUTCDateToLocalDate(new Date(overview['beginDate'])))}
                    </Typography>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.endDate')}</Typography>
                    <Box sx={{ width: '50%' }}>
                        <Typography>
                            {formatDateTime(convertUTCDateToLocalDate(new Date(overview['endDate'])))}
                        </Typography>
                        {parseInt(overview['remainHours']) > 0 && <Chip size="small" color="success" label={
                            <Remain totalHours={parseInt(overview['remainHours'])} sx={{ fontSize: 'small' }}>
                            </Remain>
                        }>
                        </Chip>}
                    </Box>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('task.assignToUser')}</Typography>
                    <Box sx={{ display: 'flex', width: '50%' }}>
                        <Avatar src={overview['assignedToUser']['avatarUrl']}></Avatar>
                        <Box sx={{ ml: 1 }}>
                            <Typography fontSize='small'>{overview['assignedToUser']['firstName']} {overview['assignedToUser']['lastName']}</Typography>
                            <Typography fontSize='small'>{overview['assignedToUser']['email']}</Typography>
                        </Box>
                    </Box>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.createdUser')}</Typography>
                    <Box sx={{ display: 'flex', width: '50%' }}>
                        <Avatar src={overview['createdUser']['avatarUrl']}></Avatar>
                        <Box sx={{ ml: 1 }}>
                            <Typography fontSize='small'>{overview['createdUser']['firstName']} {overview['createdUser']['lastName']}</Typography>
                            <Typography fontSize='small'>{overview['createdUser']['email']}</Typography>
                        </Box>
                    </Box>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.status')}</Typography>
                    <Box sx={{ width: '50%' }}>
                        <Chip sx={{ backgroundColor: TaskStatusColors[parseInt(overview['status'])] }} label={t(TaskStatusLang[parseInt(overview['status'])])}></Chip>
                    </Box>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', textAlign: 'left' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('task.previousTask')}</Typography>
                    <Box sx={{ width: '50%' }}>
                        {overview?.previousTask ? <Link target="blank" href={`/tasks/${overview['previousTask']['id']}`}>{overview['previousTask']['name']}</Link>
                            : <Typography>--</Typography>}
                    </Box>
                </Box>
            </Grid>

            <Grid item xs={12} sm={12} md={12} lg={6}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 'bold', width: '50%' }}>{t('common.note')}</Typography>
                    <Typography sx={{ width: '50%' }}>
                        {overview['note']}
                    </Typography>
                </Box>
            </Grid>

        </Grid>}
    </>
}