"use server";

import { Question } from "./types";

export const getQuestions = async (tag?: string): Promise<Question[]> => {
	let url = "http://localhost:8001/api/questions";

	if (tag) {
		url += "?tag=" + encodeURIComponent(tag);
	}

	const response = await fetch(url);

	if (!response.ok) {
		throw new Error(
			`Failed to fetch questions: ${response.status} ${response.statusText}`,
		);
	}

	return response.json();
};

export const getQuestionById = async (id: string): Promise<Question> => {
	const url = `http://localhost:8001/api/questions/${id}`;

	const response = await fetch(url);

	if (!response.ok) {
		throw new Error(
			`Failed to fetch questions: ${response.status} ${response.statusText}`,
		);
	}

	return response.json();
};
