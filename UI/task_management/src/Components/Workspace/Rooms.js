import { Box, InputAdornment, List, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import SearchIcon from '@mui/icons-material/Search';
import { useContext, useEffect, useState } from "react";
import { bearerRequest, endpoints } from "../../Config/API";
import Room from "../Room/Room";
import RoomList from "../Room/RoomList";
import { Outlet, useNavigate } from "react-router-dom";
import { APIContext, SocketContext } from "../../Common/Contexts";

export default function Rooms() {

    //#region States
    const [t] = useTranslation();
    const [rooms, setRooms] = useState([]);
    const nav = useNavigate();
    const { socket, isConnected } = useContext(SocketContext);
    const { request, bearerRequest } = useContext(APIContext);
    //#endregion

    //#region Methods
    async function loadRooms() {
        try {
            const res = await bearerRequest.get(endpoints['getJoinedRooms']);
            setRooms(res['data']['result']);
        } catch (error) {

        }
    }

    function onRoomClick(e, data) {
        nav(`/workspace/rooms/${data['code']}`);
    }

    //#endregion

    //#region Hooks
    useEffect(() => {
        loadRooms();
    }, []);

    useEffect(() => {

    });
    //#endregion

    return <>
        <Box sx={{ display: 'flex', height: '100vh' }}>
            <Box sx={{ minWidth: '200px', borderRight: '1px solid #ddd' }}>
                <Box sx={{ p: 1 }}>
                    <TextField
                        id="input-with-icon-textfield"
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <SearchIcon />
                                </InputAdornment>
                            ),
                        }}
                        variant="outlined" size="small"
                        placeholder={t('common.search')}
                    />
                </Box>
                <RoomList rooms={rooms} onItemClick={onRoomClick}></RoomList>
            </Box>
            <Box sx={{ flexGrow: 1 }}>
                <Outlet></Outlet>
            </Box>
        </Box>
    </>
}