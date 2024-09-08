import { breadcrumbsClasses } from "@mui/material";
import { act } from "react";
import cookie from 'react-cookies';

export function NotificationReducer(current, action) {
    switch (action['type']) {
        case 'show':
            return [...current, action['payload']];
        case 'close':
            return current.filter(p => p['id'] !== action['payload']['id']);
        default:
            break;
    }
}

export function BackdropReducer(current, action) {
    switch (action['type']) {
        case 'show':
            return true;
        case 'hide':
            return false;
        default:
            break;
    }
}

export function UserReducer(current, action) {
    switch (action['type']) {
        case 'login':
            return action['payload'];
        case 'logout':
            cookie.remove('access_token', { path: '/' });
            cookie.remove('refresh_token', { path: '/' });
            cookie.remove('user', { path: '/' });
            return null;
        default:
            break;
    }
}

export function SocketReducer(current, action) {
    switch (action['type']) {
        case 'connected':
            return action['payload'];
        default:
            break;
    }
}