import { getTags } from "@/lib/tag-action";
import TagCard from "./TagCard";
import TagPageHeader from "./TagsHeader";

const Page = async () => {
	const { data: tags, error } = await getTags();

	if (error) throw error;

	return (
		<div className="w-full px-6">
			<TagPageHeader />
			<div className="grid grid-cols-3 gap-4">
				{tags?.map((tag) => (
					<TagCard tag={tag} key={tag.id} />
				))}
			</div>
		</div>
	);
};

export default Page;
