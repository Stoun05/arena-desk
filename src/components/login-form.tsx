"use client";

import { Eye, EyeOff, Gamepad2, LockKeyhole, ShieldCheck, UserRoundCog } from "lucide-react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { authenticateDemoUser, demoAccounts, loadDemoSession, saveDemoSession } from "@/lib/demo-auth";

export function LoginForm() {
  const router = useRouter();
  const [email, setEmail] = useState(demoAccounts[0].email);
  const [password, setPassword] = useState(demoAccounts[0].password);
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (loadDemoSession()) router.replace("/");
  }, [router]);

  const selectAccount = (index: number) => {
    setEmail(demoAccounts[index].email);
    setPassword(demoAccounts[index].password);
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

          <div className="demo-warning"><strong>Demo login</strong><span>Bu diňe frontend synagydyr. Hakyky autentifikasiýa backend bilen indiki tapgyrda goşular.</span></div>

          <div className="account-switcher" aria-label="Demo hasaby saýla">
            <button type="button" className={email === demoAccounts[0].email ? "account-option selected" : "account-option"} onClick={() => selectAccount(0)}><UserRoundCog size={17} /><span><strong>Administrator</strong><small>Ähli mümkinçilikler</small></span></button>
            <button type="button" className={email === demoAccounts[1].email ? "account-option selected" : "account-option"} onClick={() => selectAccount(1)}><Gamepad2 size={17} /><span><strong>Kassir</strong><small>Gündelik işler</small></span></button>
          </div>

          <form className="login-form" onSubmit={(event) => {
            event.preventDefault();
            setError("");
            setIsSubmitting(true);
            const user = authenticateDemoUser(email, password);
            if (!user) {
              setError("Email ýa-da parol nädogry.");
              setIsSubmitting(false);
              return;
            }
            saveDemoSession(user);
            router.replace("/");
          }}>
            <label>Email<input type="email" value={email} onChange={(event) => setEmail(event.target.value)} autoComplete="username" required /></label>
            <label>Parol<span className="password-field"><input type={showPassword ? "text" : "password"} value={password} onChange={(event) => setPassword(event.target.value)} autoComplete="current-password" minLength={6} required /><button type="button" onClick={() => setShowPassword((value) => !value)} aria-label={showPassword ? "Paroly gizle" : "Paroly görkez"}>{showPassword ? <EyeOff size={17} /> : <Eye size={17} />}</button></span></label>
            {error && <p className="form-error" role="alert">{error}</p>}
            <button className="primary-button login-submit" type="submit" disabled={isSubmitting}>{isSubmitting ? "Girilýär..." : "Dashboard-a gir"}</button>
          </form>

          <p className="demo-credentials">Administrator: <code>admin@arena.demo</code> / <code>admin123</code><br />Kassir: <code>cashier@arena.demo</code> / <code>cashier123</code></p>
        </div>
      </section>
    </main>
  );
}
