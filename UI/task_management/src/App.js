import './App.css';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { Backdrop, CircularProgress, createTheme, ThemeProvider } from '@mui/material';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Auth from './Components/User/Auth';
import { useEffect, useReducer, useState, useTransition } from 'react';
import { APIContext, BackdropContext, BackdropDispatchContext, NotificationContext, NotificationDispatchContext, NotificationDispathContext, SocketContext, UserContext, UserDispatchContext } from './Common/Contexts';
import { BackdropReducer, NotificationReducer, SocketReducer, UserReducer } from './Common/Reducers';
import NotificationManager from './Utils/NotificationManager';
import NotificationContainer from './Components/Common/NotificationContainer';
import BackdropManager from './Utils/BackdropManager';
import API, { bearerRequest, endpoints, setBackdropDispatch } from './Config/API'
import StartPage from './Components/Workspace/StartPage';
import cookie from 'react-cookies';
import { socket } from './socket';
import FiberManualRecordIcon from '@mui/icons-material/FiberManualRecord';
import Rooms from './Components/Workspace/Rooms';
import { Chat } from '@mui/icons-material';
import ChatBox from './Components/Chat/ChatBox';
import Tasks from './Components/Chat/ChatBox';
import { useTranslation } from 'react-i18next';

function App() {

    //#region States
    const [themeMode, setThemeMode] = useState('light');
    const [notifications, notificationDispatch] = useReducer(NotificationReducer, []);
    const [showBackdrop, backdropDispatch] = useReducer(BackdropReducer, false);
    const [user, userDispatch] = useReducer(UserReducer, cookie.load('user'));
    const [isSocketAuth, setSocketAuth] = useState(false);
    const [t] = useTranslation();
    //#endregion

    //#region Initialize
    const backdropManager = BackdropManager(backdropDispatch);
    const notificationManager = NotificationManager(notifications, notificationDispatch);
    const api = API(notificationManager, t);
    //#endregion

    //#region Properties
    const defaultTheme = createTheme({
        palette: {
            mode: themeMode
        }
    });
    //#endregion

    //#region Methods
    function onConnect() {
        const accessToken = cookie.load('access_token');
        socket.emit('authenticate', { accessToken: accessToken });
    }

    function authenticated() {

    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        socket.on('connect', onConnect);
    }, []);
    //#endregion

    return (
        <SocketContext.Provider value={{ socket, isSocketAuth }}>
            <NotificationContext.Provider value={{ notifications, notificationManager }}>
                <BackdropContext.Provider value={{ showBackdrop, backdropManager }}>
                    <UserContext.Provider value={{ user, userDispatch }}>
                        <APIContext.Provider value={{...api}}>
                            <ThemeProvider theme={defaultTheme}>
                                <BrowserRouter>
                                    <Routes>
                                        <Route path='users/auth' element={<Auth></Auth>}></Route>
                                        <Route path='workspace' element={<StartPage></StartPage>}>
                                            <Route path='rooms' element={<Rooms></Rooms>}>
                                                <Route path=':code' element={<Tasks></Tasks>}></Route>
                                            </Route>
                                        </Route>
                                    </Routes>
                                </BrowserRouter>
                                <NotificationContainer></NotificationContainer>
                                <Backdrop
                                    sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
                                    open={showBackdrop} >
                                    <CircularProgress color="inherit" />
                                </Backdrop>
                            </ThemeProvider>
                        </APIContext.Provider>
                    </UserContext.Provider>
                </BackdropContext.Provider>
            </NotificationContext.Provider>
        </SocketContext.Provider >
    );
}

export default App;
