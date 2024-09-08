import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import Button from '@mui/material/Button';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import ListItem from '@mui/material/ListItem';
import { Avatar, Container, Grid, ListItemButton, Menu, MenuItem, Typography } from '@mui/material';
import TaskAltIcon from '@mui/icons-material/TaskAlt';
import GroupIcon from '@mui/icons-material/Group';
import TuneIcon from '@mui/icons-material/Tune';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { useContext, useEffect, useState } from 'react';
import { SocketContext, UserContext } from '../../Common/Contexts';
import { Outlet, useNavigate } from 'react-router-dom';
import FiberManualRecordIcon from '@mui/icons-material/FiberManualRecord';
import { bearerRequest, endpoints } from '../../Config/API';
import cookie from 'react-cookies';

export default function StartPage() {
    //#region States
    const [anchorElUser, setAnchorElUser] = useState(null);
    const { user, userDispatch } = useContext(UserContext);
    const nav = useNavigate();
    const { socket, isConnected } = useContext(SocketContext);

    //#endregion

    //#region Methods
    function handleOpenUserMenu(event) {
        setAnchorElUser(event.currentTarget);
    };

    function handleCloseUserMenu() {
        setAnchorElUser(null);
    };

    function handleLogout() {
        handleCloseUserMenu();
        userDispatch({ type: 'logout' });
        nav('/users/auth');
    }

    function onGroupBtnClick() {
        nav('/workspace/rooms');
    }

    function onAuthenticate() {
        
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        if (!user)
            window.location = '/users/auth';
    }, []);
    //#endregion


    return <>{user && <Box sx={{ display: 'flex' }}>
        <Box sx={{
            width: '70px', height: '100vh', bgcolor: 'primary.main',
            display: 'flex', flexDirection: 'column', alignItems: 'center',
            justifyContent: 'space-between'
        }} >

            <Box sx={{ position: 'relative' }}>
                <Avatar src={user['avatarUrl']} sx={{ mt: 1, cursor: 'pointer' }} onClick={handleOpenUserMenu}></Avatar>
                <FiberManualRecordIcon sx={{
                    position: 'absolute', bottom: '-5px', right: '-5px',
                    color: isConnected ? '#388E3C' : '#D32F2F', fontSize: '20px'
                }}></FiberManualRecordIcon>
            </Box>

            <Menu sx={{ ml: '55px' }} id="menu-appbar" anchorEl={anchorElUser}
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
                open={Boolean(anchorElUser)} onClose={handleCloseUserMenu}
            >
                <MenuItem >
                    <Typography textAlign="center" fontWeight='bold'>
                        {user['firstName']} {user['lastName']}
                    </Typography>
                </MenuItem>
                <MenuItem onClick={handleCloseUserMenu}>
                    <Typography textAlign="center">Hồ sơ của tôi</Typography>
                </MenuItem>
                <Divider />
                <MenuItem onClick={handleLogout}>
                    <Typography textAlign="center">Đăng xuất</Typography>
                </MenuItem>
            </Menu>

            <List sx={{ flexGrow: 1, width: '70px' }}>
                <ListItem sx={{ color: '#fff' }} disablePadding>
                    <ListItemButton>
                        <TaskAltIcon sx={{ ml: 1 }}></TaskAltIcon>
                    </ListItemButton>
                </ListItem>
                <ListItem sx={{ color: '#fff' }} disablePadding>
                    <ListItemButton onClick={onGroupBtnClick}>
                        <GroupIcon sx={{ ml: 1 }} ></GroupIcon>
                    </ListItemButton>
                </ListItem>
            </List>

            <List sx={{ width: '70px' }}>
                <ListItem sx={{ color: '#fff' }} disablePadding>
                    <ListItemButton>
                        <NotificationsIcon sx={{ ml: 1 }}></NotificationsIcon>
                    </ListItemButton>
                </ListItem>
                <ListItem sx={{ color: '#fff' }} disablePadding>
                    <ListItemButton>
                        <TuneIcon sx={{ ml: 1 }}></TuneIcon>
                    </ListItemButton>
                </ListItem>
            </List>
        </Box>

        <Box sx={{ flexGrow: 1 }}>
            <Outlet></Outlet>
        </Box>
    </Box>}</>
}