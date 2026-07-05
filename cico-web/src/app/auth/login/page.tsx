"use client";

import { useState } from "react";
import Link from "next/link";
import { ChevronRight } from "lucide-react";
import { api } from "@/lib/api-client";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const data = await api.post<{
        accessToken: string;
        refreshToken: string;
        userId?: string;
        role?: string;
        email?: string;
      }>("/auth/login", { email, password });

      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      localStorage.setItem("userId", data.userId ?? "");
      localStorage.setItem("userRole", data.role ?? "");
      localStorage.setItem("userEmail", data.email ?? email);

      window.location.href = "/";
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Login failed"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex h-screen overflow-hidden bg-white">
      <div className="hidden lg:flex w-1/2 bg-black items-center justify-center p-20">
        <h1 className="font-heading font-black text-white text-[12vw] leading-[0.8] tracking-tighter">
          CICO<br />SYSTEM
        </h1>
      </div>
      <div className="w-full lg:w-1/2 flex flex-col p-8 lg:p-20 justify-center">
        <div className="max-w-md w-full mx-auto flex flex-col gap-12">
          <header className="flex flex-col gap-2">
            <h2 className="font-heading font-black text-4xl tracking-tight">LOGIN</h2>
            <p className="text-xs opacity-40 font-heading tracking-swiss uppercase">Access your workstation</p>
          </header>

          <form className="flex flex-col gap-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-50 border border-red-200 p-4 text-xs font-black tracking-swiss text-red-800 uppercase">
                {error}
              </div>
            )}
            <div className="flex flex-col gap-2">
              <label className="text-[10px] font-black tracking-swiss opacity-40 uppercase">Email Address</label>
              <input
                type="email"
                placeholder="employee@cico.com"
                className="swiss-input"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="flex flex-col gap-2">
              <label className="text-[10px] font-black tracking-swiss opacity-40 uppercase">Password</label>
              <input
                type="password"
                placeholder="********"
                className="swiss-input"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="swiss-button w-full mt-4 flex items-center justify-between"
            >
              <span>{loading ? "Signing In..." : "Enter System"}</span>
              <ChevronRight className="w-6 h-6" />
            </button>
          </form>

          <footer className="flex flex-col gap-4 text-center">
            <p className="text-xs opacity-40">
              Don't have an account?{" "}
              <Link href="/auth/register" className="font-black text-black underline underline-offset-4">
                Request Access
              </Link>
            </p>
            <div className="h-px bg-border-muted w-1/4 mx-auto" />
            <p className="text-[8px] opacity-20 tracking-swiss uppercase">© 2026 CICO Swiss Solutions</p>
          </footer>
        </div>
      </div>
    </div>
  );
}
