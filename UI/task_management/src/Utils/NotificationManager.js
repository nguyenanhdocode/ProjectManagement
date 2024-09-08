import { v4 as uuidv4 } from 'uuid';

export default function NotificationManager(notifications, dispatch) {
    
    const TOAST_DURATION = 3000;

    return {
        showError: function (message) {
            const toast = {
                id: uuidv4(),
                type: 'toast',
                message: message,
                duration: TOAST_DURATION,
                severity: 'error',
                createdDate: new Date()
            };

            dispatch({
                type: 'show',
                payload: toast
            });
        },

        showWarning: function (message) {
            const toast = {
                id: uuidv4(),
                type: 'toast',
                message: message,
                duration: TOAST_DURATION,
                severity: 'warning',
                createdDate: new Date()
            };

            dispatch({
                type: 'show',
                payload: toast
            });
        },

        showSuccess: function (message) {
            const toast = {
                id: uuidv4(),
                type: 'toast',
                message: message,
                duration: TOAST_DURATION,
                severity: 'success',
                createdDate: new Date()
            };

            dispatch({
                type: 'show',
                payload: toast
            });
        },

        showInfo: function (message) {
            const toast = {
                id: uuidv4(),
                type: 'toast',
                message: message,
                duration: TOAST_DURATION,
                severity: 'info',
                createdDate: new Date()
            };

            dispatch({
                type: 'show',
                payload: toast
            });
        },

        hide: function(id) {
            dispatch({
                type: 'hide',
                payload: { id: id }
            });
        },

        close: function(id) {
            dispatch({
                type: 'close',
                payload: { id: id }
            });
        },

        showNotification: function(title, message, icon, link) {
            const notification = {
                id: uuidv4(),
                type: 'notification',
                title: title,
                message: message,
                duration: TOAST_DURATION,
                createdDate: new Date()
            };

            dispatch({
                type: 'show',
                payload: notification
            });
        }
    };
}