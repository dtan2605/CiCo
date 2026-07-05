"use client";

import { useState } from "react";
import Link from "next/link";
import { ChevronRight, ArrowLeft } from "lucide-react";
import { api } from "@/lib/api-client";

export default function RegisterPage() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      await api.post("/auth/register", {
        email,
        password,
        fullName: `${firstName} ${lastName}`,
        employeeCode: "",
      });
      window.location.href = "/auth/login";
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Registration failed"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex h-screen overflow-hidden bg-white">
      <div className="w-full lg:w-1/2 flex flex-col p-8 lg:p-20 justify-center relative">
        <Link
          href="/auth/login"
          className="absolute top-8 left-8 lg:top-20 lg:left-20 flex items-center gap-2 text-[10px] font-black tracking-swiss uppercase opacity-40 hover:opacity-100 transition-opacity"
        >
          <ArrowLeft className="w-4 h-4" /> Back to Login
        </Link>

        <div className="max-w-md w-full mx-auto flex flex-col gap-12">
          <header className="flex flex-col gap-2">
            <h2 className="font-heading font-black text-4xl tracking-tight">REGISTER</h2>
            <p className="text-xs opacity-40 font-heading tracking-swiss uppercase">New Employee Onboarding</p>
          </header>

          <form className="flex flex-col gap-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-50 border border-red-200 p-4 text-xs font-black tracking-swiss text-red-800 uppercase">
                {error}
              </div>
            )}
            <div className="grid grid-cols-2 gap-4">
              <div className="flex flex-col gap-2">
                <label className="text-[10px] font-black tracking-swiss opacity-40 uppercase">First Name</label>
                <input
                  type="text"
                  placeholder="John"
                  className="swiss-input"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  required
                />
              </div>
              <div className="flex flex-col gap-2">
                <label className="text-[10px] font-black tracking-swiss opacity-40 uppercase">Last Name</label>
                <input
                  type="text"
                  placeholder="Doe"
                  className="swiss-input"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  required
                />
              </div>
            </div>
            <div className="flex flex-col gap-2">
              <label className="text-[10px] font-black tracking-swiss opacity-40 uppercase">Professional Email</label>
              <input
                type="email"
                placeholder="j.doe@cico.com"
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
              <span>{loading ? "Creating..." : "Create Account"}</span>
              <ChevronRight className="w-6 h-6" />
            </button>
          </form>

          <footer className="text-center">
            <p className="text-[8px] opacity-20 tracking-swiss uppercase">Registration requires admin approval</p>
          </footer>
        </div>
      </div>

      <div className="hidden lg:flex w-1/2 bg-surface items-center justify-center p-20">
        <div className="relative group">
          <div className="absolute -inset-4 border-2 border-black opacity-10 group-hover:opacity-100 transition-opacity" />
          <h1 className="font-heading font-black text-black text-[12vw] leading-[0.8] tracking-tighter">
            JOIN<br />OUR<br />TEAM
          </h1>
        </div>
      </div>
    </div>
  );
}
