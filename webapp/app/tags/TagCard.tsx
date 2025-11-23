"use client";
import { Tag } from "@/lib/types";
import { Card, CardBody, CardFooter, CardHeader, Chip } from "@heroui/react";
import Link from "next/link";

type Props = {
	tag: Tag;
};

const TagCard = ({ tag }: Props) => {
	return (
		<Card
			as={Link}
			href={`/questions?tag=${tag.slug}`}
			isHoverable
			isPressable
		>
			<CardHeader>
				<Chip variant="bordered">{tag.slug}</Chip>
			</CardHeader>
			<CardBody>
				<p className="line-clamp-3">{tag.description}</p>
				<CardFooter>97 questions</CardFooter>
			</CardBody>
		</Card>
	);
};

export default TagCard;
