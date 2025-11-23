"use server";

import { fetchClient } from "./fetchClient";
import { Tag } from "./types";

export const getTags = async () => {
	return await fetchClient<Tag[]>("/tags", "GET", {
		cache: "force-cache",
		next: { revalidate: 3 },
	});
};
