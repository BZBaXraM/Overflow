import { getQuestions } from "@/lib/question-action";
import QuestionCard from "./QuestionCard";
import QuestionHeader from "./QuestionHeader";

const QuestionsPage = async ({
	searchParams,
}: {
	searchParams?: Promise<{ tag?: string }>;
}) => {
	const params = await searchParams;
	const questions = await getQuestions(params?.tag);
	return (
		<>
			<QuestionHeader total={questions.length} tag={params?.tag} />
			{questions.map((item) => (
				<div
					key={item.id}
					className="py-4 not-last:border-b w-full flex"
				>
					<QuestionCard key={item.id} question={item} />
				</div>
			))}
		</>
	);
};

export default QuestionsPage;
