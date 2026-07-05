"use client";

import { useState, useEffect, useCallback } from "react";
import { Send, ChevronLeft, ChevronRight } from "lucide-react";
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

type ScheduleMaster = {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  isActive: boolean;
};

const STATUS_LABELS: Record<number, string> = { 0: "Pending", 1: "Approved", 2: "Rejected" };

export default function RequestsPage() {
  const [userId, setUserId] = useState<string | null>(null);
  const [employeeId, setEmployeeId] = useState<string | null>(null);
  const [requests, setRequests] = useState<ReqItem[]>([]);
  const [masterSchedules, setMasterSchedules] = useState<ScheduleMaster[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [page, setPage] = useState(1);
  const [form, setForm] = useState({ requestDate: "", requestedScheduleId: "", reason: "" });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => { setUserId(localStorage.getItem("userId")); }, []);

  const fetchEmployee = useCallback(async () => {
    if (!userId) return;
    try {
      const emp = await api.get<{ id: string }>(`/Employees/by-user/${userId}`);
      setEmployeeId(emp.id);
    } catch { setError("Failed to load employee profile"); }
  }, [userId]);

  const fetchData = useCallback(async () => {
    if (!employeeId) return;
    setLoading(true);
    try {
      const [reqList, masters] = await Promise.all([
        api.get<ReqItem[]>(`/schedule-requests/my?pageNumber=${page}&pageSize=20`),
        api.get<ScheduleMaster[]>("/Schedules"),
      ]);
      setRequests(reqList);
      setMasterSchedules(masters);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, [employeeId, page]);

  useEffect(() => { fetchEmployee(); }, [fetchEmployee]);
  useEffect(() => { fetchData(); }, [fetchData]);

  const handleSubmit = async () => {
    if (!employeeId || !form.requestDate || !form.requestedScheduleId) return;
    setSaving(true);
    setError("");
    try {
      await api.post("/schedule-requests", {
        employeeId,
        requestDate: form.requestDate,
        currentScheduleId: null,
        requestedScheduleId: form.requestedScheduleId,
        reason: form.reason || "Shift change request",
      });
      setShowForm(false);
      setForm({ requestDate: "", requestedScheduleId: "", reason: "" });
      fetchData();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Failed to submit request");
    } finally { setSaving(false); }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <header className="flex items-center justify-between">
        <div className="flex flex-col gap-2">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            Shift Requests
          </p>
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            My Requests
          </h1>
        </div>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-black text-white px-4 py-2 text-sm font-mono hover:opacity-80 transition-opacity flex items-center gap-2 cursor-pointer"
        >
          <Send className="w-4 h-4" />
          {showForm ? "Cancel" : "New Request"}
        </button>
      </header>

      {error && (
        <div className="border border-black/20 p-4 text-sm font-mono">
          <span className="opacity-40">ERROR: </span>{error}
        </div>
      )}

      {showForm && (
        <section className="border border-black/10 p-6 lg:p-10 flex flex-col gap-6">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">Request Shift Change</p>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Date *</span>
              <input type="date" value={form.requestDate} onChange={(e) => setForm({ ...form, requestDate: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Requested Shift *</span>
              <select value={form.requestedScheduleId} onChange={(e) => setForm({ ...form, requestedScheduleId: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 bg-white">
                <option value="">--</option>
                {masterSchedules.filter((s) => s.isActive).map((s) => (
                  <option key={s.id} value={s.id}>{s.name} ({s.startTime?.slice(0, 5)}-{s.endTime?.slice(0, 5)})</option>
                ))}
              </select>
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Reason</span>
              <input type="text" value={form.reason} onChange={(e) => setForm({ ...form, reason: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" placeholder="Optional" />
            </label>
          </div>
          <div className="flex justify-end">
            <button onClick={handleSubmit} disabled={saving} className="bg-black text-white px-6 py-2 text-sm font-mono hover:opacity-80 disabled:opacity-30 cursor-pointer">
              {saving ? "Submitting..." : "Submit Request"}
            </button>
          </div>
        </section>
      )}

      <section className="flex flex-col">
        <div className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
          <span>Date</span>
          <span>Requested Shift</span>
          <span>Reason</span>
          <span>Status</span>
          <span className="text-right">Note</span>
        </div>
        {loading ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-20">Loading...</div>
        ) : requests.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-30">No requests yet</div>
        ) : requests.map((r) => (
          <div key={r.id} className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors">
            <div>{r.requestDate}</div>
            <div>{r.requestedScheduleName || "--"}</div>
            <div className="truncate opacity-60">{r.reason}</div>
            <div>
              <span className={cn(
                "inline-block px-2 py-0.5 text-[10px] font-black tracking-swiss uppercase",
                r.status === 0 ? "bg-black/5 text-black/60" : "",
                r.status === 1 ? "bg-black text-white" : "",
                r.status === 2 ? "bg-black/10 text-black/40" : "",
              )}>
                {STATUS_LABELS[r.status] || "Unknown"}
              </span>
            </div>
            <div className="text-right text-[11px] opacity-40">{r.adminNote || "--"}</div>
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
