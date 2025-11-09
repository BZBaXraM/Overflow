import { getQuestionById } from "@/lib/question-action";
import { notFound } from "next/navigation";
import QuestionDetailedHeader from "./QuestionDetailedHeader";
import QuestionContent from "./QuestionContent";
import AnswerContent from "./AnswerContent";
import AnswersHeader from "./AnswersHeader";

type Params = Promise<{ id: string }>;

const QuestionDetailsPage = async ({ params }: { params: Params }) => {
	const { id } = await params;
	const question = await getQuestionById(id);

	if (!question) return notFound();

	return (
		<div className="w-full">
			<QuestionDetailedHeader question={question} />
			<QuestionContent question={question} />
			{question.answers.length > 0 && (
				<AnswersHeader answerCount={question.answers.length} />
			)}
			{question.answers.map((item) => (
				<AnswerContent answer={item} key={item.id} />
			))}
		</div>
	);
};

export default QuestionDetailsPage;
