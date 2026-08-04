"use client";

import { type FormEvent, useState } from "react";
import { Eye, EyeOff, Loader2, LogIn, ShieldCheck } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { login } from "@/services";

type FormErrors = {
  username?: string;
  password?: string;
  form?: string;
};

export function LoginForm() {
  const router = useRouter();
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [isPending, setIsPending] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const username = String(formData.get("username") ?? "").trim();
    const password = String(formData.get("password") ?? "");
    const nextErrors: FormErrors = {};

    if (username.length < 2) {
      nextErrors.username = "Ulanyjy ady azyndan 2 harp bolmaly.";
    }

    if (password.length < 8) {
      nextErrors.password = "Parol azyndan 8 belgi bolmaly.";
    }

    setErrors(nextErrors);

    if (Object.keys(nextErrors).length > 0) {
      return;
    }

    setIsPending(true);
    try {
      await login(username, password);
      router.push("/dashboard");
    } catch (error) {
      setErrors({
        form: error instanceof TypeError
          ? "Backend bilen baglanyşyk ýok. Lokal API-ni işlediň."
          : error instanceof Error
            ? error.message
            : "Backend bilen baglanyşyk şowsuz.",
      });
    } finally {
      setIsPending(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5" noValidate>
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

      {errors.form ? (
        <p role="alert" className="rounded-xl border border-red-400/20 bg-red-400/[0.07] px-3 py-2.5 text-xs text-red-200">
          {errors.form}
        </p>
      ) : null}

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
            placeholder="Azyndan 8 belgi"
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

      <div className="rounded-xl border border-emerald-400/15 bg-emerald-400/[0.06] p-3 text-xs leading-5 text-emerald-100/80">
        <p className="flex items-center gap-2 font-medium text-emerald-200">
          <ShieldCheck aria-hidden="true" className="size-4" />
          Hakyky JWT giriş · Stage 8
        </p>
        <p className="mt-1">Lokal demo: <b>admin / Admin123!</b> ýa-da <b>cashier / Cashier123!</b></p>
      </div>

      <div className="grid grid-cols-2 gap-2">
        <Button asChild type="button" variant="outline" className="border-white/10 bg-white/[0.03] text-slate-300">
          <Link href="/dashboard?demo=admin">Admin UI demo</Link>
        </Button>
        <Button asChild type="button" variant="outline" className="border-white/10 bg-white/[0.03] text-slate-300">
          <Link href="/dashboard?demo=cashier">Kassir UI demo</Link>
        </Button>
      </div>
    </form>
  );
}
