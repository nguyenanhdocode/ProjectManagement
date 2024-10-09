import { Box, Grid, Paper, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import TodayTasks from "./TodayTasks";

export default function DashboardIndex() {

    const { t } = useTranslation();

    return <Box sx={{ p: 1 }}>
        <Typography sx={{ mb: 1 }} variant="h6">Dashboard</Typography>

        <Grid container>
            <Grid item xs={12} sm={12} md={6}>
                <Paper sx={{ p: 2}}>
                    <Typography sx={{ fontWeight: 'bold' }} variant="subtitle1">{t('dashboard.todayTasks')}</Typography>
                    <Box sx={{ overflowY: 'scroll', maxHeight: 300 }}>
                        <TodayTasks sx={{ mt: 1 }}></TodayTasks>
                    </Box>
                </Paper>
            </Grid>
        </Grid>
    </Box>
}