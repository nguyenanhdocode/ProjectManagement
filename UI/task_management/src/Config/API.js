import axios, { Axios } from "axios";
import cookie from 'react-cookies'
import { API_BASE_URL } from "./AppSettings";

export default function API(notificationManager, translate) {
    const request = axios.create({
        baseURL: API_BASE_URL
    });

    request.interceptors.response.use(res => {
        return res;
    }, error => {
        if (error?.code && error.code === 'ERR_NETWORK') {
            notificationManager.showError(translate('common.networkError'));
            throw error;
        }

        if (error.response.status === 500) {
            notificationManager.showError(translate('common.e500'));
        }

        throw {
            code: error.response.status,
            errors: error.response.data.errors
        };
    });

    const bearerRequest = axios.create({
        baseURL: API_BASE_URL
    });

    bearerRequest.interceptors.request.use(config => {
        const accessToken = cookie.load('access_token');
        config.headers.Authorization = `Bearer ${accessToken}`;
        return config;
    });

    bearerRequest.interceptors.response.use(res => {
        return res;
    }, async error => {
        if (error?.code && error.code === 'ERR_NETWORK') {
            notificationManager.showError(translate('common.networkError'));
            throw error;
        }

        const originalRequest = error.config;

        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                const refreshToken = cookie.load('refresh_token');
                const userId = cookie.load('user')?.id;
                const res = await axios.create({ baseURL: API_BASE_URL })
                    .post(endpoints['refreshToken'], { userId, refreshToken });
                cookie.save('access_token', res.data.result['accessToken'], {path: '/'});
                originalRequest.headers.Authorization = `Bearer ${res.data.result['accessToken']}`;

                return axios(originalRequest);
            } catch (error) {
                window.location = '/users/auth';
            }
        }
    });

    return {
        request,
        bearerRequest
    }
}

export const endpoints = {
    'checkEmailExisted': 'api/users/{0}',
    'createUser': 'api/users',
    'login': 'api/users/authenticate',
    'getProfile': 'api/users/profile',
    'getJoinedRooms': 'api/users/me/joined-rooms',
    'refreshToken': 'api/users/refresh-token/authenticate'
};
