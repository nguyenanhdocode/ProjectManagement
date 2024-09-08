import { useContext } from "react";
import { NotificationContext, NotificationDispatchContext } from "../../Common/Contexts";
import Toast from "./Toast";
import '../Common/NotificationContainer.css';
import { Box } from "@mui/material";
import Notification from "./Notification";

export default function NotificationContainer () {
    //#region States
    const {notifications, notifyManager} = useContext(NotificationContext);
    //#endregion

    //#region Methods
    //#endregion

    //#region Hooks
    //#endregion

    return <Box id="toast-container" sx={{p: 1}}>
        {notifications.map(p => {
            if (p['type'] === 'toast') {
                return <Toast key={p['id']} data={p}></Toast>
            } 
            else {
                return <Notification key={p['id']}  data={p}></Notification>
            }
        })}
    </Box>
}