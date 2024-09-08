import { Avatar, Box, Button, Card, CardActions, CardContent, Grid, Stack, Typography } from "@mui/material";
import { NotificationContext } from "../../Common/Contexts";
import { useContext } from "react";
import { Image } from "@mui/icons-material";

export default function Notification({ data }) {

    //#region States
    const {_, notificationManager} = useContext(NotificationContext);
    //#endregion

    //#region Methods
    function onClose() {
        notificationManager.hide(data['id'])
    }
    //#endregion

    //#region Hooks
    //#endregion

    return <>
        <Card sx={{ minWidth: '350px' }} variant="outlined">
            <CardContent sx={{ pb: 0 }}>
                <Stack direction='row'>
                    <Box>
                        <Avatar sx={{height: '50px', width: '50px'}} src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSaXrFMnQrS3cdGFTB-UpG-5qMGMQyybPu7xg&s"></Avatar>
                    </Box>
                    <Box sx={{pl: 1}}>
                        <Typography sx={{ fontSize: 15 }} gutterBottom>
                            {data['title']}
                        </Typography>
                        <Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
                            {data['message']}
                        </Typography>
                    </Box>
                </Stack>

            </CardContent>
            <CardActions sx={{ pl: 0 }}>
                <Button size="small" onClick={onClose}>Đóng</Button>
                <Button size="small">Xem chi tiết</Button>
            </CardActions>
        </Card>
    </>
}