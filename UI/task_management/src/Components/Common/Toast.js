import { useContext, useEffect, useState } from "react"
import { NotificationContext } from '../../Common/Contexts';
import { Alert } from "@mui/material";
import CheckIcon from '@mui/icons-material/Check';

export default function Toast({ data }) {
    //#region States
    const {_, notificationManager} = useContext(NotificationContext);
    //#endregion

    //#region Hooks
    useEffect(() => {
        setTimeout(() => {
            notificationManager.close(data['id'])
        }, data.duration);
    }, []);
    //#endregion

    return <>
        <Alert severity={data['severity']} sx={{mb: 1, minWidth: '350px', }}>
            {data['message']}
        </Alert>
    </>
}
