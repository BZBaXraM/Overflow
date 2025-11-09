"use client";
import { HeroUIProvider } from "@heroui/react";
import { ThemeProvider } from "next-themes";
import { useRouter } from "next/navigation";
import { ReactNode } from "react";

const Providers = ({ children }: { children: ReactNode }) => {
	const router = useRouter();

	return (
		<HeroUIProvider navigate={router.push} className="h-full flex flex-col">
			<ThemeProvider attribute={"class"} defaultTheme="light">
				{children}
			</ThemeProvider>
		</HeroUIProvider>
	);
};

export default Providers;
