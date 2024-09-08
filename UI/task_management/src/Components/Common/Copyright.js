import { Link, Typography } from "@mui/material";

export default function Copyright(props) {

    //#region States
    //#endregion

    //#region Methods
    //#endregion

    //#region Hooks
    //#endregion

    return (
        <Typography variant="body2" color="text.secondary" align="center" {...props}>
            {'Copyright © '}
            <Link color="inherit" href="https://mui.com/">
                Your Website
            </Link>{' '}
            {new Date().getFullYear()}
            {'.'}
        </Typography>
    );
}