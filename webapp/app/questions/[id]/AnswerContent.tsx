"use client";

import { Answer } from "@/lib/types";
import VottingButtons from "./VottingButtons";
import AnswerFooter from "./AnswerFooter";

type Props = {
	answer: Answer;
};

const AnswerContent = ({ answer }: Props) => {
	return (
		<div className="flex border-b pb-3 px-6">
			<VottingButtons accepted={answer.accepted} />
			<div className="flex flex-col">
				<div
					className="flex-1 mt-4 ml-6 prose max-w-none dark:prose-invert"
					dangerouslySetInnerHTML={{ __html: answer.content }}
				/>
				<AnswerFooter answer={answer} />
			</div>
		</div>
	);
};

export default AnswerContent;
