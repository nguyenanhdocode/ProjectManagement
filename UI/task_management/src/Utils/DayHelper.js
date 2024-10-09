import dayjs from "dayjs";
import utc from 'dayjs/plugin/utc';
import tz from 'dayjs/plugin/timezone';
import { rules } from "../Config/AppSettings";

export function formatDateTime(date) {
    return dayjs(date).format(rules.dateTimeFormat);
}

export function formatDate(date) {
    return dayjs(date).format(rules.dateFormat);
}

export function convertToUTC(date) {
    return dayjs(date, { utc: true }).format();
}

export function convertUTCDateToLocalDate(date) {
    var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

    var offset = date.getTimezoneOffset() / 60;
    var hours = date.getHours();

    newDate.setHours(hours - offset);

    return newDate;
}

export function convertHoursToRemain(totalHours) {
    const hoursInADay = 24;
    const hoursInAMonth = 30 * hoursInADay; // Giả sử 1 tháng có 30 ngày
    const minutesInAnHour = 60;

    const months = Math.floor(totalHours / hoursInAMonth);
    totalHours %= hoursInAMonth;

    const days = Math.floor(totalHours / hoursInADay);
    totalHours %= hoursInADay;

    const hours = Math.floor(totalHours);
    const minutes = Math.floor((totalHours - hours) * minutesInAnHour);

    return {
        months,
        days,
        hours,
        minutes
    };
}
