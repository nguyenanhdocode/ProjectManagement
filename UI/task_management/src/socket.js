import { io } from "socket.io-client";
import { SOCKET_DOMAIN } from "./Config/AppSettings";
import cookie from 'react-cookies';

export const socket = io(SOCKET_DOMAIN, {
    extraHeaders: {
        Authorization: `Bearer ${cookie.load('access_token')}`
    }
});
