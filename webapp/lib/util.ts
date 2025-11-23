import { addToast } from "@heroui/react";
import {
	differenceInCalendarDays,
	differenceInCalendarMonths,
	differenceInCalendarWeeks,
	formatDistanceToNow,
	isToday,
	isYesterday,
} from "date-fns";

export const errorToast = (error: { message: string; status?: number }) => {
	return addToast({
		color: "danger",
		title: error.status || "Error!",
		description: error.message || "Something went wrong",
	});
};

export const handleError = (error: { message: string; status?: number }) => {
	if (error.status === 500) {
		throw error;
	} else {
		return errorToast(error);
	}
};

export const fuzzyTimeAgo = (date: string | Date) => {
	const now = new Date();
	if (isToday(date)) return "Today";
	if (isYesterday(date)) return "Yesterday";

	const days = differenceInCalendarDays(now, date);
	if (days < 7) return `${days} day${days > 1 ? "s" : ""}`;

	const weeks = differenceInCalendarWeeks(now, date);
	if (weeks < 4) return `${weeks} week${weeks > 1 ? "s" : ""}`;

	const months = differenceInCalendarMonths(now, date);
	return `${months} month${months > 1 ? "s" : ""}`;
};

export const timeAgo = (date: string | Date) => {
	return formatDistanceToNow(date, { addSuffix: true });
};
