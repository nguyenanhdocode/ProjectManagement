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
import cookie from 'react-cookies';
import FiberManualRecordIcon from '@mui/icons-material/FiberManualRecord';
import { Chat } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import StartPage from './Components/Start/StartPage';
import ProjectIndex from './Components/Project/ProjectIndex';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'
import { LocalizationProvider } from '@mui/x-date-pickers';
import { viVN } from '@mui/x-date-pickers/locales';
import 'dayjs/locale/vi';
import ProjectDetail from './Components/Project/ProjectDetail';
import TaskDetail from './Components/Task/TaskDetail';
import DashboardIndex from './Components/Dashboard/DashboardIndex';

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

    //#endregion

    //#region Hooks
    useEffect(() => {
    }, []);
    //#endregion

    return (
        <LocalizationProvider dateAdapter={AdapterDayjs} 
        localeText={viVN.components.MuiLocalizationProvider.defaultProps.localeText}
        adapterLocale='vi'>
            <NotificationContext.Provider value={{ notifications, notificationManager }}>
                <BackdropContext.Provider value={{ showBackdrop, backdropManager }}>
                    <UserContext.Provider value={{ user, userDispatch }}>
                        <APIContext.Provider value={{ ...api }}>
                            <ThemeProvider theme={defaultTheme}>
                                <BrowserRouter>
                                    <Routes>
                                        <Route path='users/auth' element={<Auth></Auth>}></Route>
                                        <Route path='/' element={<StartPage></StartPage>}>
                                            <Route path='' element={<DashboardIndex></DashboardIndex>}></Route>
                                            <Route path='projects' element={<ProjectIndex></ProjectIndex>}></Route>
                                            <Route path='projects/:id' element={<ProjectDetail></ProjectDetail>}></Route>
                                            <Route path='tasks/:id' element={<TaskDetail></TaskDetail>}></Route>
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
        </LocalizationProvider>
    );
}

export default App;
