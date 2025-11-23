"use client";

import { triggerError } from "@/lib/error-action";
import { handleError } from "@/lib/util";
import { Button } from "@heroui/react";
import { useState, useTransition } from "react";

const ErrorButtons = () => {
	const [pending, startTransition] = useTransition();
	const [target, setTarget] = useState(0);
	const codes = [400, 401, 403, 404, 500];

	const onClick = (code: number) => {
		setTarget(code);
		startTransition(async () => {
			const { error } = await triggerError(code);
			if (error) handleError(error);
			setTarget(0);
		});
	};

	return (
		<div className="flex gap-6 items-center mt-6 w-full justify-center">
			{codes.map((code) => (
				<Button
					onPress={() => onClick(code)}
					color="primary"
					key={code}
					type="button"
					isLoading={pending && target === code}
				>
					Test {code}
				</Button>
			))}
		</div>
	);
};

export default ErrorButtons;
