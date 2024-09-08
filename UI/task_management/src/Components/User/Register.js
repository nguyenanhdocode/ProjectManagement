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
import { Backdrop, CircularProgress, Divider, FormControl, FormHelperText, IconButton, InputAdornment, InputLabel, LinearProgress, OutlinedInput } from '@mui/material';
import GoogleIcon from '../../Assets/Images/google.png';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import * as yup from "yup"
import { yupResolver } from '@hookform/resolvers/yup';
import { rules } from '../../Config/AppSettings';
import { useForm } from 'react-hook-form';
import { useContext, useState } from 'react';
import { APIContext, BackdropDispatchContext } from '../../Common/Contexts';
import { endpoints } from '../../Config/API';
import { LoadingButton } from '@mui/lab';

export default function Register() {

    //#region States
    const [t] = useTranslation();
    const [showPassword, setShowPassword] = useState(false);
    const [isProcessing, setProcessing] = useState(false);
    const {request, bearerRequest} = useContext(APIContext);

    const schema = yup.object({
        email: yup.string()
            .matches(rules.email, t('user.email.pattern')),
        firstName: yup.string()
            .required(t('user.firstName.required'))
            .max(rules.user.firstName.maxLength, t('user.firstName.maxLength')),
        lastName: yup.string()
            .required(t('user.lastName.required'))
            .max(rules.user.firstName.maxLength, t('user.lastName.maxLength')),
        password: yup.string()
            .required(t('user.password.required'))
            .matches(rules.user.password.pattern, t('user.password.pattern')),
    }).required();

    const {
        register,
        handleSubmit,
        watch,
        setError,
        formState: { errors },
    } = useForm({ resolver: yupResolver(schema) });

    //#endregion

    //#region Methods
    const handleClickShowPassword = () => setShowPassword((show) => !show);

    const handleMouseDownPassword = (event) => {
        event.preventDefault();
    };

    async function onSubmit(data) {
        setProcessing(true);
        if (await isEmailExisted(data['email'])) {
            setError('email', { type: 'custom', message: t('user.email.existed') });
            return;
        }

        await create(data);
    }

    async function isEmailExisted(email) {
        try {
            const res = await request.head(endpoints['checkEmailExisted'].replace('{0}', email));
            return true;
        } catch (error) {
            return false;
        }
    }

    async function create(data) {
        try {
            const res = await request.post(endpoints['createUser'], data);
            setProcessing(false);
        } catch (error) {
            setProcessing(false);
        }
    }

    //#endregion

    //#region Hooks
    //#endregion

    return (
        <Box sx={{
            display: 'flex', flexDirection: 'column',
            alignItems: 'center', mt: 1
        }}>
            <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                <LockOutlinedIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
                {t('user.signUp')}
            </Typography>
            <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate sx={{ mt: 1 }}>
                <TextField margin="normal" required fullWidth id="firstName"
                    label={t('user.firstName')} name="firstName" autoFocus
                    {...register('firstName')} error={Boolean(errors?.firstName)}
                    helperText={errors?.firstName?.message} />
                <TextField margin="normal" sx={{ mt: 1 }} required fullWidth id="lastName"
                    label={t('user.lastName')} name="lastName" autoFocus
                    {...register('lastName')} error={Boolean(errors?.lastName)}
                    helperText={errors?.lastName?.message} />
                <TextField margin="normal" sx={{ mt: 1 }} required fullWidth id="email"
                    label={t('user.email')} name="email" autoComplete="email"
                    {...register('email')} error={Boolean(errors?.email)}
                    helperText={errors?.email?.message} />
                <FormControl sx={{ mt: 1 }} fullWidth variant="outlined" required>
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
                        label="Password" {...register('password')} error={Boolean(errors?.password)}
                    />
                    <FormHelperText error={Boolean(errors?.password)}>{errors?.password?.message}</FormHelperText>
                </FormControl>
                <small>{t('user.passwordHint')}</small>
                <LoadingButton loading={isProcessing}
                    type="submit" fullWidth variant="contained" sx={{ mt: 3, mb: 2 }}>
                    {t('user.signUp')}
                </LoadingButton>
                <Grid container>
                    <Grid item xs>
                        <Link href="#" variant="body2">
                            {t('user.haveAccount')}
                        </Link>
                    </Grid>
                </Grid>
                <Divider sx={{ mt: 2 }}></Divider>
                <Button fullWidth component="label" variant="outlined"
                    tabIndex={-1} startIcon={<img src={GoogleIcon} />}
                    sx={{ mt: 2 }}>
                    {t('user.continueGoogle')}
                </Button>
            </Box>

            <Copyright sx={{ mt: 2 }}></Copyright>
        </Box>
    );
}