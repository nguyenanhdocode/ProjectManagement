import { LoadingButton } from "@mui/lab";
import { Autocomplete, Avatar, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, List, ListItem, ListItemButton, Menu, MenuItem, Paper, TextField, Typography } from "@mui/material";
import { useContext, useEffect, useReducer, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import { Controller } from "react-hook-form";
import { endpoints } from "../../Config/API";


export default function FindUserDialog({ onCancel, onDone }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [users, setUsers] = useState([]);
    const kwRef = useRef();
    const [kw, setKw] = useState();
    const [isShowSearchResult, setShowSerachResult] = useState(false);
    const [selectedUsers, setSelectedUsers] = useState([]);
    //#endregion

    //#region Methods
    function onSubmit(data) {

    }

    async function findUsers() {
        try {
            const url = `${endpoints['findUsers']}?kw=${kw}`;
            const res = await bearerRequest.get(url);
            const data = res.data.result;
            setUsers(data);
        } catch (error) {

        }
    }

    function onSearchBlur() {
        setShowSerachResult(false);
    }

    function onSearchFocus() {
        setShowSerachResult(true);
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        const timer = setTimeout(() => {
            findUsers();
        }, 500);

        return () => clearTimeout(timer);
    }, [kw]);
    //#endregion

    return <>
        <Dialog open={true}
            maxWidth='sm' fullWidth={true} sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('user.findUser')}</DialogTitle>

            <DialogContent sx={{ pt: 0 }}>
                <Autocomplete
                    multiple
                    noOptionsText={t('common.noResult')}
                    id="tags-outlined"
                    options={users}
                    getOptionLabel={(option) => option.email}
                    filterSelectedOptions
                    renderOption={(props, option) => {
                        return <MenuItem {...props}>
                            <Avatar src={option['avatarUrl']}></Avatar>
                            <Box sx={{ pl: 1 }}>
                                <Typography>{option['firstName']} {option['lastName']}</Typography>
                                <Typography sx={{ color: 'text.disabled' }}>{option['email']}</Typography>
                            </Box>
                        </MenuItem>
                    }}
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            label={t('user.findUser')}
                            onChange={(e) => setKw(e.target.value)}
                            onBlur={onSearchBlur}
                            variant="filled"
                        />
                    )}
                    sx={{ mt: 1 }}
                    onChange={(e, value) => {
                        setSelectedUsers(value);
                    }}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancel}>{t('common.cancel')}</Button>
                <LoadingButton type='submit' onClick={() => onDone(selectedUsers)}>OK</LoadingButton>
            </DialogActions>

        </Dialog>
    </>
}