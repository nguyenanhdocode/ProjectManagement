import { Accordion, AccordionDetails, AccordionSummary, Avatar, Box, Chip, Paper, Typography } from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

export default function TodayTasks({ ...props }) {
    return <>
        <Box {...props}>
            <Box>
                <Typography variant="subtitle1" fontWeight='bold' color='text.secondary'
                    sx={{ borderBottom: '1px solid', borderBottomColor: 'text.disabled' }}>
                    Dự án số 1
                </Typography>
                <Box>
                    <Paper sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', p: 1, mt: 1 }}>
                        <Box sx={{ textAlign: 'left', flexGrow: 1, pl: 1 }}>
                            <Typography variant="body1">Viết báo cáo</Typography>
                            <Box sx={{ display: 'flex' }}>
                                <Typography color='primary' fontSize='small' sx={{ cursor: 'pointer' }}>Tổng quan</Typography>
                                <Typography color='primary' sx={{ ml: 1, cursor: 'pointer' }} fontSize='small'>Xem chi tiết</Typography>
                            </Box>
                        </Box>
                        <Chip size="small" color="success" label="Chưa thực hiện"></Chip>
                    </Paper>
                </Box>
            </Box>
        </Box>
    </>
}