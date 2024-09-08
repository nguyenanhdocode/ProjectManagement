import * as React from 'react';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import Copyright from '../Common/Copyright';
import Login from './Login';
import { Card, Tab, Tabs } from '@mui/material';
import CustomTabPanel from '../Common/CustomTabPanel';
import { useTranslation } from 'react-i18next';
import Register from './Register';
import { useParams, useSearchParams } from 'react-router-dom';

export default function Auth() {

    //#region States
    const [tabIndex, setTabIndex] = React.useState(0);
    const [t] = useTranslation();
    const [params, setParams] = useSearchParams();
    //#endregion

    //#region Methods
    function a11yProps(index) {
        return {
            id: `simple-tab-${index}`,
            'aria-controls': `simple-tabpanel-${index}`,
        };
    }

    const handleChange = (event, newValue) => {
        params.set('tab', newValue);
        setParams(params);
    };
    //#endregion

    //#region Hooks
    React.useEffect(() => {
        setTabIndex(parseInt(params.get('tab') ?? 0));
    }, [params.get('tab')]);
    //#endregion

    return (
        <Container component="main" maxWidth="xs">
            <Card sx={{px: 2, pb: 1, mt: 2}}>
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Tabs value={tabIndex} onChange={handleChange} variant='fullWidth'>
                        <Tab label={t('user.signIn')} {...a11yProps(0)} />
                        <Tab label={t('user.signUp')} {...a11yProps(1)} />
                    </Tabs>
                </Box>
                <CustomTabPanel value={tabIndex} index={0}>
                    <Login></Login>
                </CustomTabPanel>
                <CustomTabPanel value={tabIndex} index={1}>
                    <Register></Register>
                </CustomTabPanel>
            </Card>
        </Container>
    );
}