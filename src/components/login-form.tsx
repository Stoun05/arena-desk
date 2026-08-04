"use client";

import { Eye, EyeOff, Gamepad2, LockKeyhole, ShieldCheck, UserRoundCog } from "lucide-react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { login } from "@/lib/api-client";

const accountPresets = [
  { email: "admin@arena.local", label: "Administrator", detail: "Ähli mümkinçilikler" },
  { email: "cashier@arena.local", label: "Kassir", detail: "Gündelik işler" },
];

export function LoginForm() {
  const router = useRouter();
  const [email, setEmail] = useState(accountPresets[0].email);
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const selectAccount = (index: number) => {
    setEmail(accountPresets[index].email);
    setPassword("");
    setError("");
  };

  return (
    <main className="login-page">
      <section className="login-visual" aria-label="ArenaDesk mümkinçilikleri">
        <div className="login-brand"><span className="brand-mark"><Gamepad2 size={22} /></span><span><strong>ArenaDesk</strong><small>Gaming club manager</small></span></div>
        <div className="login-message">
          <p className="eyebrow">KIBERKLUB DOLANDYRYŞY</p>
          <h1>Ähli kompýuterler bir panelde.</h1>
          <p>Sessiýalary, wagtlary, kassirleri we kompýuterleriň ýagdaýyny bir ýerden dolandyr.</p>
        </div>
        <div className="login-feature"><ShieldCheck size={19} /><span><strong>Rol boýunça giriş</strong><small>Administrator we kassir üçin aýratyn hukuklar</small></span></div>
      </section>

      <section className="login-panel">
        <div className="login-card">
          <div className="login-card-head"><span className="login-lock"><LockKeyhole size={21} /></span><div><p className="eyebrow">HOŞ GELDIŇIZ</p><h2>Ulgama giriş</h2></div></div>

          <div className="demo-warning"><strong>Howpsuz giriş</strong><span>Parol serverde hash görnüşinde barlanýar. Sessiya HttpOnly cookie arkaly saklanýar.</span></div>

          <div className="account-switcher" aria-label="Demo hasaby saýla">
            <button type="button" className={email === accountPresets[0].email ? "account-option selected" : "account-option"} onClick={() => selectAccount(0)}><UserRoundCog size={17} /><span><strong>{accountPresets[0].label}</strong><small>{accountPresets[0].detail}</small></span></button>
            <button type="button" className={email === accountPresets[1].email ? "account-option selected" : "account-option"} onClick={() => selectAccount(1)}><Gamepad2 size={17} /><span><strong>{accountPresets[1].label}</strong><small>{accountPresets[1].detail}</small></span></button>
          </div>

          <form className="login-form" onSubmit={async (event) => {
            event.preventDefault();
            setError("");
            setIsSubmitting(true);
            try {
              await login(email, password);
              router.replace("/");
            } catch (loginError) {
              setError(loginError instanceof Error ? loginError.message : "Servere birigip bolmady.");
              setIsSubmitting(false);
            }
          }}>
            <label>Email<input type="email" value={email} onChange={(event) => setEmail(event.target.value)} autoComplete="username" required /></label>
            <label>Parol<span className="password-field"><input type={showPassword ? "text" : "password"} value={password} onChange={(event) => setPassword(event.target.value)} autoComplete="current-password" minLength={6} required /><button type="button" onClick={() => setShowPassword((value) => !value)} aria-label={showPassword ? "Paroly gizle" : "Paroly görkez"}>{showPassword ? <EyeOff size={17} /> : <Eye size={17} />}</button></span></label>
            {error && <p className="form-error" role="alert">{error}</p>}
            <button className="primary-button login-submit" type="submit" disabled={isSubmitting}>{isSubmitting ? "Girilýär..." : "Dashboard-a gir"}</button>
          </form>

          <p className="demo-credentials">Ilkinji parollar server işe girizilende <code>.env</code> arkaly bellenýär.</p>
        </div>
      </section>
    </main>
  );
}
