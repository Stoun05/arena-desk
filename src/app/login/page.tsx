import type { Metadata } from "next";
import { LoginForm } from "@/components/login-form";

export const metadata: Metadata = {
  title: "Giriş | ArenaDesk",
  description: "ArenaDesk demo giriş sahypasy",
};

export default function LoginPage() {
  return <LoginForm />;
}
