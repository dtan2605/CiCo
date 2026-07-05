"use client";

import { useState, useEffect, useCallback } from "react";
import { Check, X, Clock, User, Mail, Phone, Home } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type ProfileRequest = {
  id: string;
  employeeId: string;
  employeeName: string;
  employeeCode: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: string;
  status: string;
  createdAt: string;
};

export default function AdminProfileRequestsPage() {
  const [requests, setRequests] = useState<ProfileRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [resolving, setResolving] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const list = await api.get<ProfileRequest[]>("/profile-update-requests/pending");
      setRequests(list);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleResolve = async (id: string, approve: boolean) => {
    setResolving(id);
    try {
      await api.put(`/profile-update-requests/${id}/resolve`, { approve });
      setRequests((prev) => prev.filter((r) => r.id !== id));
    } catch { /* ignore */ }
    finally { setResolving(null); }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — REQUESTS
        </p>
        <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
          Profile Update Requests
        </h1>
      </section>

      <section className="flex flex-col">
        {loading ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-20">Loading...</div>
        ) : requests.length === 0 ? (
          <div className="py-16 text-center">
            <p className="font-heading font-black text-2xl tracking-tight opacity-20">
              NO PENDING REQUESTS
            </p>
            <p className="text-sm font-mono opacity-30 mt-2">
              All profile update requests have been resolved.
            </p>
          </div>
        ) : requests.map((r) => (
          <div
            key={r.id}
            className="border border-black/10 p-8 mb-6 flex flex-col lg:flex-row lg:items-start gap-8"
          >
            <div className="flex-1 grid grid-cols-1 sm:grid-cols-2 gap-6">
              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <User className="w-3 h-3" /> Employee
                </span>
                <span className="font-heading font-black text-lg tracking-tight">
                  {r.employeeName}
                </span>
                <span className="text-xs font-mono opacity-40">{r.employeeCode}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <Clock className="w-3 h-3" /> Submitted
                </span>
                <span className="font-mono text-sm">
                  {new Date(r.createdAt).toLocaleString()}
                </span>
              </div>

              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <User className="w-3 h-3" /> Requested Name
                </span>
                <span className="font-heading font-black text-lg">{r.fullName}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <Mail className="w-3 h-3" /> Requested Email
                </span>
                <span className="font-mono text-sm">{r.email}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <Phone className="w-3 h-3" /> Requested Phone
                </span>
                <span className="font-mono text-sm">{r.phoneNumber || "—"}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-[10px] font-black tracking-swiss opacity-30 uppercase flex items-center gap-1">
                  <Home className="w-3 h-3" /> Requested Address
                </span>
                <span className="font-mono text-sm">{r.address || "—"}</span>
              </div>
            </div>
            <div className="flex gap-3 shrink-0">
              <button
                onClick={() => handleResolve(r.id, true)}
                disabled={resolving === r.id}
                className="swiss-button text-xs flex items-center gap-2 disabled:opacity-40"
              >
                <Check className="w-4 h-4" />
                {resolving === r.id ? "..." : "Approve"}
              </button>
              <button
                onClick={() => handleResolve(r.id, false)}
                disabled={resolving === r.id}
                className="swiss-button-outline text-xs flex items-center gap-2 disabled:opacity-40"
              >
                <X className="w-4 h-4" />
                Reject
              </button>
            </div>
          </div>
        ))}
      </section>
    </main>
  );
}
