import { Box, ListItem, ListItemButton, Typography } from "@mui/material";
import GroupsIcon from '@mui/icons-material/Groups';

export default function Room({ data, onItemClick }) {
    return <ListItem disablePadding>
        <ListItemButton onClick={(e) => {
            if (typeof(onItemClick) === 'function')
                onItemClick(e, data);
        }}>
            <GroupsIcon></GroupsIcon>
            <Box>
                <Typography sx={{ pl: 1 }}>{data['name']}</Typography>
                <Typography sx={{ pl: 1 }} fontSize='small' color=''>Đô Nguyễn: Hello</Typography>
            </Box>
        </ListItemButton>
    </ListItem >
}