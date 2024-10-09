import { Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { convertHoursToRemain } from "../../Utils/DayHelper";

export default function Remain({ totalHours, showMinutes, ...props }) {

    const [t] = useTranslation();

    const monthLang = t('common.remain.month');
    const monthsLang = t('common.remain.months');
    const dayLang = t('common.remain.day');
    const daysLang = t('common.remain.days');
    const hourLang = t('common.remain.hour');
    const hoursLang = t('common.remain.hours');
    const minuteLang = t('common.remain.minute');
    const minutesLang = t('common.remain.minutes');

    const remains = convertHoursToRemain(totalHours);
    let remainStr = '';
    if (remains.months > 0)
        remainStr = remainStr.concat(` ${remains.months} ${remains.months < 2 ? monthLang : monthsLang}`);

    if (remains.days > 0)
        remainStr = remainStr.concat(` ${remains.days} ${remains.days < 2 ? dayLang : daysLang}`);

    if (remains.hours > 0)
        remainStr = remainStr.concat(` ${remains.hours} ${remains.hours < 2 ? hourLang : hoursLang}`);

    if (showMinutes && remains.minutes > 0)
        remainStr = remainStr.concat(` ${remains.minutes} ${remains.minutes < 2 ? minuteLang : minutesLang}`);

    return <Typography {...props}>
        {remainStr.trim()}
    </Typography>
}