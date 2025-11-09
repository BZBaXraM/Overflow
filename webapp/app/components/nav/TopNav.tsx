"use client";
import {
	AcademicCapIcon,
	MagnifyingGlassIcon,
} from "@heroicons/react/24/solid";
import { Button, Input } from "@heroui/react";
import Link from "next/link";
import ThemeToggle from "./ThemeToggle";

const TopNav = () => {
	return (
		<header className="p-2 w-full fixed top-0 z-50 bg-white dark:bg-black">
			<div className="flex px-10 mx-auto">
				<div className="flex items-center gap-6">
					<Link
						href={"/"}
						className="flex items-center gap-3 max-h-16"
					>
						<AcademicCapIcon className="size-10 text-secondary" />
						<h3 className="text-xl font-semibold uppercase">
							Overflow
						</h3>
					</Link>
					<nav className="flex gap-3 my-2 text-md text-neutral-500">
						<Link href={"/"}>Bahram</Link>
						<Link href={"/"}>Push</Link>
						<Link href={"/"}>Back</Link>
					</nav>
				</div>

				<Input
					startContent={<MagnifyingGlassIcon className="size-6" />}
					className="ml-6"
					type="search"
					placeholder="Search"
				/>

				<div className="flex basis-1/4 shrink-0 justify-end gap-3">
					<ThemeToggle />
					<Button color="secondary" variant="bordered">
						Login
					</Button>
					<Button color="secondary">Register</Button>
				</div>
			</div>
		</header>
	);
};

export default TopNav;
