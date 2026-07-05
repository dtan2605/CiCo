"use client";

import { useState, useEffect, useCallback } from "react";
import { Check, X, ChevronLeft, ChevronRight, Search } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type ReqItem = {
  id: string;
  employeeId: string;
  employeeName: string;
  requestDate: string;
  currentScheduleName: string | null;
  requestedScheduleName: string | null;
  reason: string;
  status: number;
  adminNote: string | null;
  createdAt: string;
  resolvedAt: string | null;
};

const STATUS_LABELS: Record<number, string> = { 0: "Pending", 1: "Approved", 2: "Rejected" };

export default function AdminRequestsPage() {
  const [requests, setRequests] = useState<ReqItem[]>([]);
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<number | undefined>(undefined);
  const [loading, setLoading] = useState(true);
  const [resolving, setResolving] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams({ pageNumber: String(page), pageSize: "20" });
      if (statusFilter !== undefined) params.set("status", String(statusFilter));
      const list = await api.get<ReqItem[]>(`/schedule-requests?${params}`);
      setRequests(list);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, [page, statusFilter]);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleResolve = async (id: string, status: number) => {
    setResolving(id);
    try {
      await api.put(`/schedule-requests/${id}/resolve`, { id, status, adminNote: null });
      fetchData();
    } catch { /* ignore */ }
    finally { setResolving(null); }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4 mt-auto">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — SHIFT REQUESTS
        </p>
        <div className="flex items-center justify-between gap-4">
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            Schedule Requests
          </h1>
          <div className="flex items-center gap-3">
            {[undefined, 0, 1, 2].map((s) => (
              <button
                key={s ?? "all"}
                onClick={() => { setStatusFilter(s); setPage(1); }}
                className={cn(
                  "text-[10px] font-black tracking-swiss uppercase px-3 py-2 border transition-colors cursor-pointer",
                  statusFilter === s ? "bg-black text-white border-black" : "border-black/10 hover:border-black/40"
                )}
              >
                {s === undefined ? "All" : STATUS_LABELS[s]}
              </button>
            ))}
          </div>
        </div>
      </section>

      <section className="flex flex-col">
        <div className="grid grid-cols-[1.5fr_1fr_1fr_1fr_0.5fr_0.5fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
          <span>Employee</span>
          <span>Date</span>
          <span>Requested Shift</span>
          <span>Reason</span>
          <span>Status</span>
          <span className="text-right">Actions</span>
        </div>
        {loading ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-20">Loading...</div>
        ) : requests.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-30">No requests found</div>
        ) : requests.map((r) => (
          <div key={r.id} className="grid grid-cols-[1.5fr_1fr_1fr_1fr_0.5fr_0.5fr] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors">
            <div>{r.employeeName}</div>
            <div className="opacity-60">{r.requestDate}</div>
            <div>{r.requestedScheduleName || "--"}</div>
            <div className="truncate opacity-60">{r.reason}</div>
            <div>
              <span className={cn(
                "inline-block px-2 py-0.5 text-[10px] font-black tracking-swiss uppercase",
                r.status === 0 ? "bg-black/5 text-black/60" : "",
                r.status === 1 ? "bg-black text-white" : "",
                r.status === 2 ? "bg-black/10 text-black/40" : "",
              )}>
                {STATUS_LABELS[r.status]}
              </span>
            </div>
            <div className="flex justify-end gap-1">
              {r.status === 0 && (
                <>
                  <button
                    onClick={() => handleResolve(r.id, 1)}
                    disabled={resolving === r.id}
                    className="p-1.5 border border-black/10 hover:bg-black hover:text-white transition-colors disabled:opacity-20 cursor-pointer"
                  >
                    <Check className="w-3.5 h-3.5" />
                  </button>
                  <button
                    onClick={() => handleResolve(r.id, 2)}
                    disabled={resolving === r.id}
                    className="p-1.5 border border-black/10 hover:bg-black hover:text-white transition-colors disabled:opacity-20 cursor-pointer"
                  >
                    <X className="w-3.5 h-3.5" />
                  </button>
                </>
              )}
              {r.status !== 0 && (
                <span className="text-[11px] opacity-40">{r.adminNote || "--"}</span>
              )}
            </div>
          </div>
        ))}
      </section>

      <div className="flex items-center justify-center gap-4 mt-auto">
        <button onClick={() => setPage(Math.max(1, page - 1))} disabled={page === 1} className="border border-black/10 px-3 py-1 text-sm font-mono hover:opacity-60 disabled:opacity-20 cursor-pointer">
          <ChevronLeft className="w-4 h-4" />
        </button>
        <span className="text-sm font-mono opacity-40">Page {page}</span>
        <button onClick={() => setPage(page + 1)} disabled={requests.length < 20} className="border border-black/10 px-3 py-1 text-sm font-mono hover:opacity-60 disabled:opacity-20 cursor-pointer">
          <ChevronRight className="w-4 h-4" />
        </button>
      </div>
    </main>
  );
}
