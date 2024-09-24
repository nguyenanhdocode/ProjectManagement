import * as React from 'react';
import { styled, useTheme } from '@mui/material/styles';
import Box from '@mui/material/Box';
import MuiDrawer from '@mui/material/Drawer';
import MuiAppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import List from '@mui/material/List';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { useTranslation } from 'react-i18next';
import { ContentPaste, Logout } from '@mui/icons-material';
import StorageIcon from '@mui/icons-material/Storage';
import { Avatar, TextField } from '@mui/material';
import { UserContext } from '../../Common/Contexts';
import Logo from '../Common/Logo';
import { Outlet, useNavigate } from 'react-router-dom';
import BarChartIcon from '@mui/icons-material/BarChart';
import cookie from 'react-cookies';

const drawerWidth = 240;

const openedMixin = (theme) => ({
    width: drawerWidth,
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.enteringScreen,
    }),
    overflowX: 'hidden',
});

const closedMixin = (theme) => ({
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    width: `calc(${theme.spacing(7)} + 1px)`,
    [theme.breakpoints.up('sm')]: {
        width: `calc(${theme.spacing(8)} + 1px)`,
    },
});

const DrawerHeader = styled('div')(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: theme.spacing(0, 1),
    height: '50px'
}));

const AppBar = styled(MuiAppBar, {
    shouldForwardProp: (prop) => prop !== 'open',
})(({ theme }) => ({
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    variants: [
        {
            props: ({ open }) => open,
            style: {
                marginLeft: drawerWidth,
                width: `calc(100% - ${drawerWidth}px)`,
                transition: theme.transitions.create(['width', 'margin'], {
                    easing: theme.transitions.easing.sharp,
                    duration: theme.transitions.duration.enteringScreen,
                }),
            },
        },
    ],
}));

const Drawer = styled(MuiDrawer, { shouldForwardProp: (prop) => prop !== 'open' })(
    ({ theme }) => ({
        width: drawerWidth,
        flexShrink: 0,
        whiteSpace: 'nowrap',
        boxSizing: 'border-box',
        variants: [
            {
                props: ({ open }) => open,
                style: {
                    ...openedMixin(theme),
                    '& .MuiDrawer-paper': openedMixin(theme),
                },
            },
            {
                props: ({ open }) => !open,
                style: {
                    ...closedMixin(theme),
                    '& .MuiDrawer-paper': closedMixin(theme),
                },
            },
        ],
    }),
);

export default function StartPage() {
    const theme = useTheme();
    const [open, setOpen] = React.useState(cookie.load('menu_opened') == 'true');
    const { t } = useTranslation();
    const { user, userDispatch } = React.useContext(UserContext);
    const nav = useNavigate();

    const handleDrawerOpen = () => {
        setOpen(true);
        cookie.save('menu_opened', true, { path: '/' });
    };

    const handleDrawerClose = () => {
        setOpen(false);
        cookie.save('menu_opened', false, { path: '/' });
    };

    function signOutClick() {
        userDispatch({ type: 'logout' })
        nav('/users/auth');
    }

    function projectMenuClick() {
        nav('/projects');
    }

    return (
        <Box sx={{ display: 'flex' }}>
            <CssBaseline />
            <AppBar position="fixed" open={open}>
                <Box sx={{ display: 'flex', alignItems: 'center', pl: 2, height: 50 }}>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        onClick={handleDrawerOpen}
                        edge="start"
                        sx={[
                            {
                               marginRight: 5
                            },
                            open && { display: 'none' },
                        ]}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Logo width='100px'></Logo>
                </Box>
            </AppBar>
            <Drawer variant="permanent" open={open}>
                <DrawerHeader>
                    <IconButton onClick={handleDrawerClose}>
                        {theme.direction === 'rtl' ? <ChevronRightIcon /> : <ChevronLeftIcon />}
                    </IconButton>
                </DrawerHeader>
                <Divider />

                <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-around' }}>
                    <List>
                        <ListItem key='profile' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                sx={[
                                    { minHeight: 48, px: 2.5 },
                                    open ? { justifyContent: 'initial' }
                                        : { justifyContent: 'center' }
                                ]}>
                                <ListItemIcon
                                    sx={[
                                        { minWidth: 0, justifyContent: 'center' },
                                        open ? { mr: 3 }
                                            : { mr: 'auto' }
                                    ]}>
                                    <Avatar src={user['avatarUrl']}></Avatar>
                                </ListItemIcon>
                                <ListItemText sx={[open ? { opacity: 1 } : { opacity: 0 }]} >
                                    <Typography sx={{ fontWeight: 'bold' }}>{user['firstName']} {user['lastName']}</Typography>
                                    <Typography variant='body2'>{user['email']}</Typography>
                                </ListItemText>
                            </ListItemButton>
                        </ListItem>
                        <ListItem key='dashboard' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                sx={[
                                    { minHeight: 48, px: 2.5 },
                                    open ? { justifyContent: 'initial' }
                                        : { justifyContent: 'center' }
                                ]}>
                                <ListItemIcon
                                    sx={[
                                        { minWidth: 0, justifyContent: 'center' },
                                        open ? { mr: 3 }
                                            : { mr: 'auto' }
                                    ]}>
                                    <BarChartIcon />
                                </ListItemIcon>
                                <ListItemText primary='Dashboard'
                                    sx={[open ? { opacity: 1 } : { opacity: 0 }]} />
                            </ListItemButton>
                        </ListItem>
                        <ListItem key='project' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                sx={[
                                    { minHeight: 48, px: 2.5 },
                                    open ? { justifyContent: 'initial' }
                                        : { justifyContent: 'center' }
                                ]} onClick={projectMenuClick}>
                                <ListItemIcon
                                    sx={[
                                        { minWidth: 0, justifyContent: 'center' },
                                        open ? { mr: 3 }
                                            : { mr: 'auto' }
                                    ]}>
                                    <ContentPaste />
                                </ListItemIcon>
                                <ListItemText primary={t('project.projects')}
                                    sx={[open ? { opacity: 1 } : { opacity: 0 }]} />
                            </ListItemButton>
                        </ListItem>
                        <ListItem key='asset' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                sx={[
                                    { minHeight: 48, px: 2.5 },
                                    open ? { justifyContent: 'initial' }
                                        : { justifyContent: 'center' }
                                ]}>
                                <ListItemIcon
                                    sx={[
                                        { minWidth: 0, justifyContent: 'center' },
                                        open ? { mr: 3 }
                                            : { mr: 'auto' }
                                    ]}>
                                    <StorageIcon />
                                </ListItemIcon>
                                <ListItemText primary={t('asset.asset')}
                                    sx={[open ? { opacity: 1 } : { opacity: 0 }]} />
                            </ListItemButton>
                        </ListItem>
                    </List>

                    <List>
                        <ListItem key='signout' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                sx={[
                                    { minHeight: 48, px: 2.5 },
                                    open ? { justifyContent: 'initial' }
                                        : { justifyContent: 'center' }
                                ]} onClick={signOutClick}>
                                <ListItemIcon
                                    sx={[
                                        { minWidth: 0, justifyContent: 'center' },
                                        open ? { mr: 3 }
                                            : { mr: 'auto' }
                                    ]}>
                                    <Logout />
                                </ListItemIcon>
                                <ListItemText primary={t('user.signOut')}
                                    sx={[open ? { opacity: 1 } : { opacity: 0 }]} />
                            </ListItemButton>
                        </ListItem>
                    </List>
                </Box>
            </Drawer>
            <Box sx={{ flexGrow: 1, minWidth: 0 }}>
                <DrawerHeader></DrawerHeader>
                <Outlet></Outlet>
            </Box>
        </Box>
    );
}