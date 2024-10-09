import { useContext, useEffect, useReducer, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { APIContext, NotificationContext } from "../../Common/Contexts";
import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, Divider, Grid, Link, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { Circle, Image } from "@mui/icons-material";
import { endpoints } from "../../Config/API";
import CloudDoneIcon from '@mui/icons-material/CloudDone';
import { ImageExtensions } from "./AssetValidatorConfiguration";
import Checkbox from '@mui/material/Checkbox';

export default function ChooseAssetsDialog({ onCancel, onDone }) {

    //#region States
    const { t } = useTranslation();
    const { request, bearerRequest } = useContext(APIContext);
    const { notificationManager } = useContext(NotificationContext);
    const [isProcessing, setProcessing] = useState(false);
    const [uploaded, setUploaded] = useState([]);
    const fileRef = useRef();
    const [files, setFiles] = useState();
    const [selectedAssets, setSelectedAssets] = useState([]);
    //#endregion

    //#region Methods
    function onFileClick() {
        fileRef.current.click();
    }

    async function onFileChange(e) {
        const fileList = Array.from(e.target.files).map(p => {
            return {
                file: p,
                uploading: true
            }
        });

        setFiles(fileList);

        for (let i = 0; i < fileList.length; i++) {

            let item = fileList[i];

            if (!item.uploading)
                continue;

            let formData = new FormData();
            formData.append('file', item['file']);

            try {
                const res = await bearerRequest.post(endpoints['createAsset'], formData);
                let newState = [...fileList];
                newState[i].uploading = false;
                setFiles(newState);
                getAssets();
            } catch (error) {

            }
        }
    }

    async function getAssets() {
        try {
            const res = await bearerRequest.get(endpoints['getAssets']);
            const data = res.data.result;
            setUploaded(data);
        } catch (error) {

        }
    }

    function onCheckedChange(e, asset) {
        if (e.target.checked) {
            setSelectedAssets(current => {
                return [...current, asset];
            });
        } else {
            setSelectedAssets(current => {
                return [...current.filter(p => p['id'] != asset['id'])];
            });
        }
    }
    //#endregion

    //#region Hooks
    useEffect(() => {
        getAssets();
    }, []);
    //#endregion

    return <>
        <Dialog open={true}
            maxWidth='lg' fullWidth={true}
            sx={{ position: 'absolute', top: 0 }}>
            <DialogTitle>{t('asset.chooseAssets')}</DialogTitle>

            <DialogContent>
                <Box>
                    <Button variant="outlined" startIcon={<CloudUploadIcon></CloudUploadIcon>}
                        onClick={onFileClick}>
                        {t('asset.upload')}
                    </Button>
                    <input type="file" multiple hidden ref={fileRef} onChange={onFileChange} />

                    <Box sx={{ mt: 1 }}>
                        {files && files.map(p => <Box sx={{ display: 'flex', mb: 1, alignItems: 'center' }}>
                            {p['uploading']
                                ? <CircularProgress size={20} color="inherit"></CircularProgress>
                                : <CloudDoneIcon color="success" fontSize="small"></CloudDoneIcon>}
                            <Typography fontSize='small' sx={{ ml: 1 }}>
                                {p['file']['name']}
                            </Typography>
                        </Box>)}
                    </Box>
                </Box>
                <Divider sx={{ mt: 1 }}></Divider>
                <Box>
                    <Typography variant="h6" sx={{ mt: 1 }}>{t('asset.uploaded')}</Typography>

                    <TableContainer>
                        <Table size="small">
                            <TableHead>
                                <TableCell>{t('common.name')}</TableCell>
                                <TableCell>{t('asset.format')}</TableCell>
                                <TableCell>{t('asset.size')}</TableCell>
                                <TableCell>{t('common.createdUser')}</TableCell>
                                <TableCell>{t('asset.selected')}</TableCell>
                            </TableHead>
                            <TableBody>
                                {uploaded && uploaded.map(p => (<TableRow>
                                    <TableCell>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <img width={24} height={24}
                                                src={ImageExtensions.includes(p['type']) ? p['url'] : `../IconsPack/${p['type']}.svg`} />

                                            <Link target="blank" href={p['url']}>
                                                <Typography sx={{ ml: 1 }} fontSize='small'>{p['displayFileName']}</Typography>
                                            </Link>
                                        </Box>
                                    </TableCell>
                                    <TableCell>{p['type']}</TableCell>
                                    <TableCell>{(parseFloat(p['size']) / 1024).toFixed(2)} KB</TableCell>
                                    <TableCell>{p['createdUser']['email']}</TableCell>
                                    <TableCell>
                                        <Checkbox onChange={(e) => onCheckedChange(e, p)} />
                                    </TableCell>
                                </TableRow>))}
                            </TableBody>
                        </Table>
                        {uploaded.length == 0 && <Typography sx={{ mt: 1 }} textAlign='center' fontSize='small'>
                            {t('asset.noAssets')}
                        </Typography>}
                    </TableContainer>
                </Box>
            </DialogContent>

            <DialogActions>
                <Button onClick={onCancel}>{t('common.cancel')}</Button>
                <Button type='submit' onClick={() => onDone(selectedAssets)}>OK</Button>
            </DialogActions>
        </Dialog>
    </>
}