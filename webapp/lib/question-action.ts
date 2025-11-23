"use server";

import { fetchClient } from "./fetchClient";
import { Question } from "./types";

export const getQuestions = async (tag?: string) => {
	let url = "/questions";

	if (tag) {
		url += "?tag=" + encodeURIComponent(tag);
	}

	return await fetchClient<Question[]>(url, "GET");
};

export const getQuestionById = async (id: string) => {
	return await fetchClient<Question>(`/questions/${id}`, "GET");
};

export const searchQuestions = async (query: string) => {
	return await fetchClient<Question[]>(`/search?query=${query}`, "GET");
};
