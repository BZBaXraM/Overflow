"use client";
import { searchQuestions } from "@/lib/question-action";
import { Question } from "@/lib/types";
import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { Input, Listbox, ListboxItem, Spinner } from "@heroui/react";
import { useEffect, useRef, useState } from "react";

const SearchInput = () => {
	const [query, setQuery] = useState("");
	const [loading, setLoading] = useState(false);
	const [results, setResults] = useState<Question[] | null>(null);
	const [showDropDown, setShowDropDown] = useState(false);
	const timeoutRef = useRef<NodeJS.Timeout | null>(null);

	useEffect(() => {
		if (timeoutRef.current) clearTimeout(timeoutRef.current);

		if (!query) {
			// eslint-disable-next-line react-hooks/set-state-in-effect
			setResults(null);
			setShowDropDown(false);
			return;
		}

		timeoutRef.current = setTimeout(async () => {
			setLoading(true);
			const { data: questions } = await searchQuestions(query);
			setResults(questions);
			setLoading(false);
			setShowDropDown(true);
		}, 300);
	}, [query]);

	const onAction = () => {
		setQuery("");
		setResults(null);
	};

	return (
		<div className="flex flex-col w-full">
			<Input
				startContent={<MagnifyingGlassIcon className="size-6" />}
				className="ml-6"
				type="search"
				placeholder="Search"
				value={query}
				onChange={(e) => setQuery(e.target.value)}
				endContent={loading && <Spinner size="sm" />}
			/>
			{showDropDown && results && (
				<div className="absolute top-full z-50 bg-white dark:bg-default-50 shadow-lg border-2 border-default-500 w-[50%]">
					<Listbox
						onAction={onAction}
						items={results}
						className="flex flex-col overflow-y-auto"
					>
						{(questions) => (
							<ListboxItem
								href={`/questions/${questions.id}`}
								key={questions.id}
								startContent={
									<div className="flex flex-col h-14 min-h-14 justify-center items-center border border-success rounded-md">
										<span>{questions.answerCount}</span>
										<span className="text-xs">answers</span>
									</div>
								}
							>
								<div className="font-semibold">
									{questions.title}
								</div>
								<div className="text-xs opacity-60 line-clamp-2">
									{questions.content}
								</div>
							</ListboxItem>
						)}
					</Listbox>
				</div>
			)}
		</div>
	);
};

export default SearchInput;
