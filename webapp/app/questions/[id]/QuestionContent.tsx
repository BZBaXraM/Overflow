"use client";

import { Question } from "@/lib/types";
import VottingButtons from "./VottingButtons";
import QuestionFooter from "./QuestionFooter";

type Props = {
	question: Question;
};

const QuestionContent = ({ question }: Props) => {
	return (
		<div className="flex border-b pb-3 px-6">
			<VottingButtons />
			<div className="flex flex-col">
				<div
					className="flex-1 mt-4 ml-6 prose dark:prose-invert max-w-none"
					dangerouslySetInnerHTML={{ __html: question.content }}
				/>
				<QuestionFooter question={question} />
			</div>
		</div>
	);
};

export default QuestionContent;
