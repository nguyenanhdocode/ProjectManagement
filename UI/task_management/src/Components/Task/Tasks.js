import { Box, Container, Grid, Stack, Typography } from "@mui/material";
import { useParams } from "react-router-dom"
import GroupsIcon from '@mui/icons-material/Groups';
import { useContext, useEffect } from "react";
import { SocketContext } from "../../Common/Contexts";

export default function Tasks() {
    //#region States
    const { code } = useParams();
    const socket = useContext(SocketContext);
    //#endregion

    //#region Methods
    //#endregion

    //#region Hooks
    useEffect(() => {
        
    }, []);
    //#endregion

    return <Box>

        <Box sx={{
            height: '50px', borderBottom: '1px solid #ddd', pl: 1,
            display: 'flex', alignItems: 'center'
        }}>
            <GroupsIcon></GroupsIcon>
            <Box sx={{ pl: 1 }}>
                <Typography>Nhóm chat</Typography>
                <Typography sx={{ fontSize: 'small' }}>12 thành viên</Typography>
            </Box>
        </Box>

    </Box>
}