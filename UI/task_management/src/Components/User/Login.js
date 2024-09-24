import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Copyright from '../Common/Copyright';
import { useTranslation } from 'react-i18next';
import { Divider, FormControl, FormHelperText, IconButton, InputAdornment, InputLabel, OutlinedInput } from '@mui/material';
import GoogleIcon from '../../Assets/Images/google.png';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { LoadingButton } from '@mui/lab';
import { useContext, useEffect, useState } from 'react';
import { APIContext, BackdropDispatchContext, NotificationContext, NotificationDispatchContext, UserContext } from '../../Common/Contexts';
import { useForm, SubmitHandler } from "react-hook-form"
import * as yup from "yup"
import { yupResolver } from '@hookform/resolvers/yup';
import cookie from 'react-cookies';
import { useNavigate } from 'react-router-dom';
import { endpoints } from '../../Config/API';
import { HttpStatusCode } from 'axios';

export default function Login() {

    //#region States
    const [t] = useTranslation();
    const [showPassword, setShowPassword] = useState(false);
    const [isProcessing, setProcessing] = useState(false);
    const {notificationManager} = useContext(NotificationContext);
    const {_, userDispatch} = useContext(UserContext);
    const nav = useNavigate();
    const {request, bearerRequest} = useContext(APIContext);

    const schema = yup.object({
        email: yup.string().required(t('user.email.required')),
        password: yup.string().required(t('user.password.required'))
    }).required();

    const {
        register,
        handleSubmit,
        watch,
        formState: { errors },
    } = useForm({ resolver: yupResolver(schema) });

    //#endregion

    //#region Methods
    async function onSubmit(data) {
        await login({
            username: data['email'],
            password: data['password']
        });
    };

    async function login(data) {
        try {
            setProcessing(true);
            const res = await request.post(endpoints['login'], data);
            const result = res['data']['result'];
            cookie.save('access_token', result['accessToken'], { path: '/' });
            cookie.save('refresh_token', result['refreshToken'], { path: '/' });
            const profile = await loadProfile();
            userDispatch({type: 'login', payload: profile});
            cookie.save('user', profile, { path: '/' });

            nav('/projects');
        } catch (error) {
            if (error.code === HttpStatusCode.Unauthorized) {
                notificationManager.showError(t('user.loginFailed'))
            }
        }
        setProcessing(false);
    }

    async function loadProfile() {
        try {
            const res = await bearerRequest.get(endpoints['getProfile']);
            return res['data']['result'];
        } catch (error) {
            return null;
        }
    }

    const handleClickShowPassword = () => setShowPassword((show) => !show);

    const handleMouseDownPassword = (event) => {
        event.preventDefault();
    };
    //#endregion

    //#region Hooks
    useEffect(() => {
        
    }, []);
    //#endregion

    return (
        <Box sx={{
            display: 'flex', flexDirection: 'column',
            alignItems: 'center', mt: 4
        }}>
            <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                <LockOutlinedIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
                {t('user.signIn')}
            </Typography>
            <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate sx={{ mt: 1 }}>
                <TextField margin="normal" required fullWidth id="email"
                    label={t('user.email')} name="email" autoComplete="email" autoFocus
                    {...register("email")} error={Boolean(errors?.email)} helperText={errors?.email?.message} />
                <FormControl fullWidth sx={{ mt: 1 }} variant="outlined" required>
                    <InputLabel error={Boolean(errors?.password)} htmlFor="outlined-adornment-password">{t('user.password')}</InputLabel>
                    <OutlinedInput type={showPassword ? 'text' : 'password'}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowPassword}
                                    onMouseDown={handleMouseDownPassword}
                                    edge="end" >
                                    {showPassword ? <VisibilityOff /> : <Visibility />}
                                </IconButton>
                            </InputAdornment>
                        }
                        {...register("password")} error={Boolean(errors?.password)} label="Password" />
                    <FormHelperText error={Boolean(errors?.password)}>{errors?.password?.message}</FormHelperText>
                </FormControl>
                <LoadingButton loading={isProcessing}
                    variant="contained"
                    type="submit" fullWidth sx={{ mt: 3, mb: 2 }}
                    onClick={handleSubmit}>
                    {t('user.signIn')}
                </LoadingButton>
                <Grid container>
                    <Grid item xs>
                        <Link href="#" variant="body2">
                            {t('user.forgotPassword')}
                        </Link>
                    </Grid>
                    <Grid item>
                        <Link href="#" variant="body2">
                            {t('user.dontHaveAccount')}
                        </Link>
                    </Grid>
                </Grid>
                <Divider sx={{ mt: 2 }}></Divider>
                <Button fullWidth component="label" role={undefined} variant="outlined"
                    tabIndex={-1} startIcon={<img src={GoogleIcon} />}
                    sx={{ mt: 2 }}>
                    {t('user.continueGoogle')}
                </Button>
            </Box>

            <Copyright sx={{ mt: 4 }}></Copyright>
        </Box>
    );
}