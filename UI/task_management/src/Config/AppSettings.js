export const SOCKET_DOMAIN = 'http://localhost:4000';
export const API_BASE_URL = 'https://localhost:7087/';

export const rules =  {
    email: {
        pattern: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/
    },
    user: {
        firstName: {
            maxLength: 35
        },
        lastName: {
            maxLength: 35
        },
        password: {
            pattern: '^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$'
        }
    },
    dateTimeFormat: 'DD/MM/YYYY HH:mm',
    dateFormat: 'DD/MM/YYYY'
}
