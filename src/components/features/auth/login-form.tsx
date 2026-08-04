"use client";

import { type FormEvent, useState, useTransition } from "react";
import { Eye, EyeOff, Loader2, LogIn } from "lucide-react";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { UserRole } from "@/types/auth";

type FormErrors = {
  username?: string;
  password?: string;
};

export function LoginForm() {
  const router = useRouter();
  const [role, setRole] = useState<UserRole>("admin");
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [isPending, startTransition] = useTransition();

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const username = String(formData.get("username") ?? "").trim();
    const password = String(formData.get("password") ?? "");
    const nextErrors: FormErrors = {};

    if (username.length < 2) {
      nextErrors.username = "Ulanyjy ady azyndan 2 harp bolmaly.";
    }

    if (password.length < 6) {
      nextErrors.password = "Parol azyndan 6 belgi bolmaly.";
    }

    setErrors(nextErrors);

    if (Object.keys(nextErrors).length > 0) {
      return;
    }

    startTransition(() => {
      router.push(`/dashboard?role=${role}`);
    });
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5" noValidate>
      <div className="space-y-2">
        <Label htmlFor="role" className="text-slate-300">
          Ulanyjy roly
        </Label>
        <Select value={role} onValueChange={(value) => setRole(value as UserRole)}>
          <SelectTrigger id="role" className="h-11 w-full border-white/10 bg-white/[0.035] text-slate-100">
            <SelectValue placeholder="Roly saýlaň" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="admin">Administrator</SelectItem>
            <SelectItem value="cashier">Kassir</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-2">
        <Label htmlFor="username" className="text-slate-300">
          Ulanyjy ady
        </Label>
        <Input
          id="username"
          name="username"
          autoComplete="username"
          placeholder="Mysal: admin"
          aria-invalid={Boolean(errors.username)}
          aria-describedby={errors.username ? "username-error" : undefined}
          className="h-11 border-white/10 bg-white/[0.035] text-white placeholder:text-slate-600"
        />
        {errors.username ? (
          <p id="username-error" role="alert" className="text-xs text-red-300">
            {errors.username}
          </p>
        ) : null}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password" className="text-slate-300">
          Parol
        </Label>
        <div className="relative">
          <Input
            id="password"
            name="password"
            type={showPassword ? "text" : "password"}
            autoComplete="current-password"
            placeholder="Azyndan 6 belgi"
            aria-invalid={Boolean(errors.password)}
            aria-describedby={errors.password ? "password-error" : undefined}
            className="h-11 border-white/10 bg-white/[0.035] pr-11 text-white placeholder:text-slate-600"
          />
          <button
            type="button"
            onClick={() => setShowPassword((current) => !current)}
            aria-label={showPassword ? "Paroly gizle" : "Paroly görkez"}
            className="absolute inset-y-0 right-0 grid w-11 place-items-center text-slate-500 transition-colors hover:text-slate-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-400"
          >
            {showPassword ? <EyeOff aria-hidden="true" className="size-4" /> : <Eye aria-hidden="true" className="size-4" />}
          </button>
        </div>
        {errors.password ? (
          <p id="password-error" role="alert" className="text-xs text-red-300">
            {errors.password}
          </p>
        ) : null}
      </div>

      <Button type="submit" size="lg" disabled={isPending} className="h-11 w-full bg-indigo-500 text-white hover:bg-indigo-400">
        {isPending ? (
          <Loader2 aria-hidden="true" data-icon="inline-start" className="animate-spin" />
        ) : (
          <LogIn aria-hidden="true" data-icon="inline-start" />
        )}
        {isPending ? "Açylýar..." : "Ulgama gir"}
      </Button>

      <div className="rounded-xl border border-amber-400/15 bg-amber-400/[0.06] p-3 text-xs leading-5 text-amber-200/80">
        Demo režimi: maglumatlar backend-e iberilmeýär. Islendik 2+ belgili
        ulanyjy adyny we 6+ belgili paroly girizip bilersiňiz.
      </div>
    </form>
  );
}
