import { getQuestions } from "@/lib/question-action";
import QuestionCard from "./QuestionCard";
import QuestionHeader from "./QuestionHeader";

const QuestionsPage = async ({
	searchParams,
}: {
	searchParams?: Promise<{ tag?: string }>;
}) => {
	const params = await searchParams;
	const { data: questions, error } = await getQuestions(params?.tag);

	if (error) throw error;

	return (
		<>
			<QuestionHeader total={questions?.length || 0} tag={params?.tag} />
			{questions?.map((item) => (
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
