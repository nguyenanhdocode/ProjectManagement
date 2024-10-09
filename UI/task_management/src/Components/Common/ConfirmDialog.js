import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { useTranslation } from "react-i18next";

export default function ConfirmDialog({ onCancel, onAgree, title, message, renderOption }) {

    const [t] = useTranslation();

    return <Dialog
        open onClose={onCancel}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description">
        <DialogTitle id="alert-dialog-title">
            {title}
        </DialogTitle>
        <DialogContent>
            {typeof renderOption == 'function'
                ? renderOption()
                : <DialogContentText id="alert-dialog-description">
                    {message}
                </DialogContentText>}
        </DialogContent>
        <DialogActions>
            <Button onClick={onCancel}>
                {t('common.cancel')}
            </Button>
            <Button onClick={onAgree} autoFocus>
                {t('common.agree')}
            </Button>
        </DialogActions>
    </Dialog>
}