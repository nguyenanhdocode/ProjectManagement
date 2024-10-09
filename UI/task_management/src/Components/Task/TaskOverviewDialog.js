import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Paper, Typography } from "@mui/material";
import { useContext } from "react";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import TaskOverview from "./TaskOverview";

export default function TaskOverviewDialog({ onClose, taskId }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    //#endregion

    //#region Methods
    async function name(params) {

    }
    //#endregion

    return <Dialog open={true} maxWidth='md' fullWidth={true} sx={{ position: 'absolute', top: 0 }}>
        <DialogTitle>{t('task.overview')}</DialogTitle>
        <DialogContent>
            <TaskOverview taskId={taskId}></TaskOverview>
        </DialogContent>
        <DialogActions>
            <Button onClick={onClose}>{t('common.close')}</Button>
        </DialogActions>
    </Dialog>
}