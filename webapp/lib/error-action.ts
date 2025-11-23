"use server";

import { fetchClient } from "./fetchClient";

export const triggerError = async (code: number) => {
	return await fetchClient(`/questions/errors?code=${code}`, "GET");
};
