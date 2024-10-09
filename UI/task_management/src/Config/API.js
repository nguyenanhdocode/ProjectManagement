import axios, { Axios, HttpStatusCode } from "axios";
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
                cookie.save('access_token', res.data.result['accessToken'], { path: '/' });
                originalRequest.headers.Authorization = `Bearer ${res.data.result['accessToken']}`;

                return axios(originalRequest);
            } catch (error) {
                window.location = '/users/auth';
            }
        }

        throw error;
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
    'refreshToken': 'api/users/refresh-token/authenticate',
    'findUsers': 'api/users',
    'getAllProjects': 'api/projects',
    'createProject': 'api/projects',
    'updateProject': 'api/projects/{0}',
    'updateProjectBeginDate': 'api/projects/{0}/begindate',
    'updateProjectEndDate': 'api/projects/{0}/enddate',
    'deleteProject': 'api/projects/{0}',
    'getProject': 'api/projects/{0}',
    'getProjectOverview': 'api/projects/{0}/overview',
    'getTimeline': 'api/projects/{0}/timeline',
    'getProjectMembers': 'api/projects/{0}/members',
    'getProjectTasks': 'api/projects/{0}/tasks',
    'checkBeforeUpdateProjectBeginDate': 'api/projects/{0}/is-valid-before-update-begindate',
    'checkBeforeUpdateProjectEndDate': 'api/projects/{0}/is-valid-before-update-enddate',
    'addProjectMembers': 'api/projects/{0}/members',
    'removeMember': 'api/projects/{0}/members/{1}',
    'createTask': 'api/tasks',
    'updateTask': 'api/tasks/{0}',
    'getTaskOverview': 'api/tasks/{0}/overview',
    'getTask': 'api/tasks/{0}',
    'checkAffectBeforeUpdateTaskEndDate': 'api/tasks/{0}/is-affect-when-update-enddate',
    'checkCanUpdateBeginDate': 'api/tasks/{0}/can-update-enddate',
    'getAllowedNewStatus': 'api/tasks/{0}/allowed-new-status',
    'deleteTask': 'api/tasks/{0}',
    'getSubtasks': 'api/tasks/{0}/subtasks',
    'createSubtask': 'api/tasks/{0}/subtasks',
    'updateSubtask': 'api/tasks/{0}/subtasks',
    'shiftTask': 'api/tasks/{0}/time',
    'createAsset': 'api/assets/files',
    'getAssets': 'api/assets',
    'addProjectAssets': 'api/projects/{0}/assets',
    'getProjectAssets': 'api/projects/{0}/assets',
    'removeAssetFromProject': 'api/projects/{0}/assets/{1}',
    'getAvaiablePrevTask': 'api/projects/{0}/avaiable-prev-tasks',
    'getProjectProgress': 'api/projects/{0}/progress',
};
