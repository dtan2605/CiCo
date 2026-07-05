"use client";

import { useState, useEffect, useCallback } from "react";
import { Download, Filter, X } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";
import { useAuth } from "@/lib/auth-context";

type AttendanceRecord = {
  id: string;
  attendanceDate: string;
  checkInTime: string | null;
  checkOutTime: string | null;
  lateMinutes: number;
  status: string;
  employeeId: string;
  employeeName?: string;
  employeeCode?: string;
};

export default function HistoryPage() {
  const { user } = useAuth();
  const [userId] = useState<string | null>(() =>
    typeof window !== "undefined" ? localStorage.getItem("userId") : null
  );
  const [employeeId, setEmployeeId] = useState<string | null>(null);
  const [employeeInfo, setEmployeeInfo] = useState<{ fullName: string; employeeCode: string } | null>(null);
  const [records, setRecords] = useState<AttendanceRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [cancelId, setCancelId] = useState<string | null>(null);
  const [securityCode, setSecurityCode] = useState("");
  const [cancelError, setCancelError] = useState("");
  const [cancelling, setCancelling] = useState(false);
  const [showFilters, setShowFilters] = useState(false);
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const role = user?.role ?? "";
  const isAdmin = role === "Admin";
  const isManager = role === "Manager";

  useEffect(() => {
    if (!userId) return;
    if (!isAdmin && !isManager) {
      api.get<{ id: string; fullName: string; employeeCode: string }>(`/Employees/by-user/${userId}`)
        .then((emp) => {
          setEmployeeId(emp.id);
          setEmployeeInfo({ fullName: emp.fullName, employeeCode: emp.employeeCode });
        })
        .catch(() => {});
    }
  }, [userId, isAdmin, isManager]);

  const buildUrl = useCallback(() => {
    if (isAdmin) {
      const params = new URLSearchParams();
      if (fromDate) params.set("fromDate", fromDate);
      if (toDate) params.set("toDate", toDate);
      const qs = params.toString();
      return `/admin/attendance${qs ? `?${qs}` : ""}`;
    }
    if (isManager) {
      const params = new URLSearchParams();
      if (fromDate) params.set("fromDate", fromDate);
      if (toDate) params.set("toDate", toDate);
      const qs = params.toString();
      return `/manager/attendance${qs ? `?${qs}` : ""}`;
    }
    if (employeeId) {
      return `/attendance/by-employee/${employeeId}`;
    }
    return null;
  }, [employeeId, isAdmin, isManager, fromDate, toDate]);

  useEffect(() => {
    const url = buildUrl();
    if (!url) { setLoading(false); return; }
    setLoading(true);
    api.get<AttendanceRecord[]>(url)
      .then(setRecords)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [buildUrl]);

  const clearFilters = () => { setFromDate(""); setToDate(""); };

  const hasFilters = fromDate || toDate;

  const calcHours = (checkIn: string | null, checkOut: string | null) => {
    if (!checkIn || !checkOut) return "--";
    const diff = new Date(checkOut).getTime() - new Date(checkIn).getTime();
    const h = Math.floor(diff / 3600000);
    const m = Math.floor((diff % 3600000) / 60000);
    return `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}`;
  };

  const handleCancel = async () => {
    if (!cancelId) return;
    setCancelling(true);
    setCancelError("");
    try {
      await api.delete(`/attendance/${cancelId}`, { attendanceId: cancelId, securityCode });
      setCancelId(null);
      setSecurityCode("");
      setRecords((prev) => prev.filter((r) => r.id !== cancelId));
    } catch (e: unknown) {
      setCancelError(e instanceof Error ? e.message : "Cancel failed");
    } finally {
      setCancelling(false);
    }
  };

  const fmt = (dt: string | null) =>
    dt
      ? new Date(dt).toLocaleTimeString("en-US", { hour12: false, hour: "2-digit", minute: "2-digit", second: "2-digit" })
      : "--";

  const getCode = (r: AttendanceRecord) =>
    isAdmin || isManager ? (r.employeeCode || "") : (employeeInfo?.employeeCode || "");
  const getName = (r: AttendanceRecord) =>
    isAdmin || isManager ? (r.employeeName || "") : (employeeInfo?.fullName || "");

  const exportCsv = () => {
    const header = "Date,Employee Code,Employee Name,Check In,Check Out,Total Hours,Status,Late Minutes";
    const rows = records.map(r => {
      const date = r.attendanceDate;
      const checkIn = r.checkInTime ? new Date(r.checkInTime).toLocaleTimeString("en-US", { hour12: false }) : "--";
      const checkOut = r.checkOutTime ? new Date(r.checkOutTime).toLocaleTimeString("en-US", { hour12: false }) : "--";
      const hours = calcHours(r.checkInTime, r.checkOutTime);
      return `${date},${getCode(r)},${getName(r)},${checkIn},${checkOut},${hours},${r.status},${r.lateMinutes}`;
    });
    const csv = [header, ...rows].join("\n");
    const blob = new Blob([csv], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `attendance-${new Date().toISOString().slice(0, 10)}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  };

  const gridCols = isAdmin ? "grid-cols-7" : "grid-cols-6";

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-16 lg:gap-24">
      <header className="flex flex-col lg:flex-row lg:items-end justify-between gap-8">
        <div className="flex flex-col gap-2">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            {isAdmin || isManager ? "All Employees" : "Attendance Log"}
          </p>
          <h1 className="font-heading font-black text-6xl tracking-tight">
            ATTENDANCE LOG
          </h1>
          {!isAdmin && !isManager && employeeInfo && (
            <p className="font-mono text-sm opacity-40 mt-1">
              {employeeInfo.employeeCode} &mdash; {employeeInfo.fullName}
            </p>
          )}
        </div>
        <div className="flex gap-4">
          <button
            onClick={() => setShowFilters(!showFilters)}
            className={cn(
              "flex items-center gap-2 border-2 border-black px-6 py-3 text-[10px] font-black tracking-swiss uppercase transition-transform active:scale-95",
              showFilters && "bg-black text-white"
            )}
          >
            <Filter className="w-4 h-4" /> Filter By Date
          </button>
          <button
            onClick={exportCsv}
            disabled={records.length === 0}
            className="flex items-center gap-2 bg-black text-white px-6 py-3 text-[10px] font-black tracking-swiss uppercase transition-transform active:scale-95 disabled:opacity-30"
          >
            <Download className="w-4 h-4" /> Export CSV
          </button>
        </div>
      </header>

      {showFilters && (
        <div className="border-2 border-black p-6 flex flex-wrap gap-4 items-end">
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">From Date</label>
            <input
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className="border-2 border-black px-3 py-2 text-sm font-mono focus:outline-none"
            />
          </div>
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">To Date</label>
            <input
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className="border-2 border-black px-3 py-2 text-sm font-mono focus:outline-none"
            />
          </div>
          {hasFilters && (
            <button
              onClick={clearFilters}
              className="flex items-center gap-2 border-2 border-black px-4 py-2 text-[10px] font-black tracking-swiss uppercase transition-transform active:scale-95"
            >
              <X className="w-3 h-3" /> Clear
            </button>
          )}
        </div>
      )}

      <section className="flex flex-col border-t border-border-muted">
        <div className={cn("grid py-6 px-4 text-[10px] font-black tracking-swiss opacity-40 uppercase", gridCols)}>
          <span>Date</span>
          <span>Name</span>
          <span>Code</span>
          <span>Check In</span>
          <span>Check Out</span>
          <span className="text-right">Status</span>
          {isAdmin && <span className="text-right">Actions</span>}
        </div>
        {loading && (
          <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
            Loading records...
          </div>
        )}
        {!loading && records.length === 0 && (
          <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
            No attendance records found
          </div>
        )}
        {records.map((log) => (
          <div
            key={log.id}
            className={cn(
              "grid py-8 px-4 border-t border-border-muted group hover:bg-surface transition-colors items-center",
              gridCols
            )}
          >
            <span className="font-heading font-black text-sm">{log.attendanceDate}</span>
            <span className="text-sm">{getName(log) || "--"}</span>
            <span className="font-mono text-xs opacity-60">{getCode(log) || "--"}</span>
            <span className="font-mono text-xs opacity-60">{fmt(log.checkInTime)}</span>
            <span className="font-mono text-xs opacity-60">{fmt(log.checkOutTime)}</span>
            <div className="flex justify-end">
              <span
                className={cn(
                  "text-[10px] font-black tracking-swiss px-3 py-1 border",
                  log.status === "Late"
                    ? "border-black bg-black text-white"
                    : log.status === "Absent"
                    ? "border-red-500 bg-red-500 text-white"
                    : "border-border-muted"
                )}
              >
                {log.status.toUpperCase()}
              </span>
            </div>
            {isAdmin && (
              <div className="flex justify-end">
                <button
                  onClick={() => { setCancelId(log.id); setSecurityCode(""); setCancelError(""); }}
                  className="text-[10px] font-mono underline opacity-0 group-hover:opacity-40 hover:opacity-100 transition-opacity cursor-pointer"
                >
                  Cancel
                </button>
              </div>
            )}
          </div>
        ))}
      </section>

      {cancelId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/20">
          <div className="bg-white p-10 flex flex-col gap-6 min-w-[360px] border border-black/10">
            <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">Cancel Attendance</p>
            <p className="text-sm font-mono opacity-60">Enter security code to cancel this record</p>
            <input
              type="password"
              value={securityCode}
              onChange={(e) => setSecurityCode(e.target.value)}
              placeholder="Security code"
              className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40"
              autoFocus
            />
            {cancelError && (
              <p className="text-sm font-mono border border-black/20 p-3">
                <span className="opacity-40">ERROR: </span>{cancelError}
              </p>
            )}
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setCancelId(null)}
                className="border border-black/10 px-4 py-2 text-sm font-mono hover:opacity-60 cursor-pointer"
              >
                Cancel
              </button>
              <button
                onClick={handleCancel}
                disabled={cancelling || !securityCode}
                className="bg-black text-white px-4 py-2 text-sm font-mono hover:opacity-80 disabled:opacity-30 cursor-pointer"
              >
                {cancelling ? "Processing..." : "Confirm"}
              </button>
            </div>
          </div>
        </div>
      )}

      <footer className="flex justify-between items-center py-10 border-t border-border-muted">
        <p className="text-[10px] opacity-30 font-medium">
          Showing {records.length} record{records.length !== 1 ? "s" : ""}
        </p>
      </footer>
    </main>
  );
}
