import { Box } from "@mui/material";

export default function CustomTabPanel(props) {
    //#region States
    const { children, value, index, ...other } = props;
    //#endregion

    //#region Methods
    //#endregion

    //#region Hooks
    //#endregion

    return (
        <div role="tabpanel" hidden={value !== index} id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`} {...other}>
            {value === index && <Box>{children}</Box>}
        </div>
    );
}