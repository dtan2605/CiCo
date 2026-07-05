"use client";

import { useState, useEffect, useCallback } from "react";
import { Plus, X, ChevronLeft, ChevronRight, Trash2 } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type EmpSchedule = {
  id: string;
  employeeId: string;
  scheduleId: string;
  workDate: string;
  isOvertime: boolean;
  note: string;
  employeeName: string;
  scheduleName: string;
  startTime: string;
  endTime: string;
};

type ScheduleMaster = {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  isActive: boolean;
  lateThresholdMinutes: number;
};

type Employee = {
  id: string;
  fullName: string;
};

export default function AdminSchedulesPage() {
  const [assignments, setAssignments] = useState<EmpSchedule[]>([]);
  const [masters, setMasters] = useState<ScheduleMaster[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [showShiftForm, setShowShiftForm] = useState(false);
  const [weekOffset, setWeekOffset] = useState(0);
  const [form, setForm] = useState({
    employeeId: "",
    scheduleId: "",
    workDate: "",
    isOvertime: false,
    note: "",
  });
  const [shiftForm, setShiftForm] = useState({ name: "", startTime: "", endTime: "", lateThreshold: "15" });
  const [saving, setSaving] = useState(false);
  const [shiftSaving, setShiftSaving] = useState(false);

  const fetchData = useCallback(async () => {
    try {
      const [scheds, empList, masterList] = await Promise.all([
        api.get<EmpSchedule[]>("/admin/schedules?pageSize=100"),
        api.get<Employee[]>("/employees?pageSize=100").catch(() => []),
        api.get<ScheduleMaster[]>("/Schedules"),
      ]);
      setAssignments(scheds);
      setEmployees(empList as Employee[]);
      setMasters(masterList);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleCreate = async () => {
    if (!form.employeeId || !form.scheduleId || !form.workDate) return;
    setSaving(true);
    try {
      await api.post("/admin/schedules", {
        employeeId: form.employeeId,
        scheduleId: form.scheduleId,
        workDate: form.workDate,
        isOvertime: form.isOvertime,
        note: form.note || null,
      });
      setShowForm(false);
      setForm({ employeeId: "", scheduleId: "", workDate: "", isOvertime: false, note: "" });
      fetchData();
    } catch { /* ignore */ }
    finally { setSaving(false); }
  };

  const handleDelete = async (id: string) => {
    try {
      await api.delete(`/admin/schedules/${id}`);
      fetchData();
    } catch { /* ignore */ }
  };

  const handleCreateShift = async () => {
    if (!shiftForm.name || !shiftForm.startTime || !shiftForm.endTime) return;
    setShiftSaving(true);
    try {
      await api.post("/Schedules", {
        name: shiftForm.name,
        startTime: shiftForm.startTime,
        endTime: shiftForm.endTime,
        lateThresholdMinutes: parseInt(shiftForm.lateThreshold) || 15,
      });
      setShowShiftForm(false);
      setShiftForm({ name: "", startTime: "", endTime: "", lateThreshold: "15" });
      fetchData();
    } catch { /* ignore */ }
    finally { setShiftSaving(false); }
  };

  const handleDeleteShift = async (id: string) => {
    try {
      await api.delete(`/Schedules/${id}`);
      fetchData();
    } catch { /* ignore */ }
  };

  const today = new Date();
  const startOfWeek = new Date(today);
  startOfWeek.setDate(today.getDate() + weekOffset * 7 - today.getDay() + 1);

  const weekDays = Array.from({ length: 7 }, (_, i) => {
    const d = new Date(startOfWeek);
    d.setDate(startOfWeek.getDate() + i);
    return d;
  });

  const weekAssignments = assignments.filter((a) => {
    const d = new Date(a.workDate);
    return d >= weekDays[0] && d <= weekDays[6];
  });

  const getAssignmentsForDay = (date: Date) => {
    const dateStr = date.toISOString().split("T")[0];
    return weekAssignments.filter((a) => a.workDate === dateStr);
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4 mt-auto">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — SCHEDULE MANAGEMENT
        </p>
        <div className="flex items-center justify-between gap-4">
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            Employee Schedules
          </h1>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-2 border border-black/10 p-2">
              <button onClick={() => setWeekOffset((w) => w - 1)} className="opacity-40 hover:opacity-100 cursor-pointer">
                <ChevronLeft className="w-4 h-4" />
              </button>
              <span className="text-[10px] font-black tracking-swiss uppercase px-2">
                {weekDays[0].toLocaleDateString()} — {weekDays[6].toLocaleDateString()}
              </span>
              <button onClick={() => setWeekOffset((w) => w + 1)} className="opacity-40 hover:opacity-100 cursor-pointer">
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
            <button
              onClick={() => setShowShiftForm(!showShiftForm)}
              className="border border-black/10 px-4 py-2 text-sm font-mono hover:opacity-60 transition-opacity flex items-center gap-2 cursor-pointer"
            >
              {showShiftForm ? <X className="w-4 h-4" /> : <Plus className="w-4 h-4" />}
              {showShiftForm ? "Cancel" : "New Shift"}
            </button>
            <button
              onClick={() => setShowForm(!showForm)}
              className="bg-black text-white px-4 py-2 text-sm font-mono hover:opacity-80 transition-opacity flex items-center gap-2 cursor-pointer"
            >
              {showForm ? <X className="w-4 h-4" /> : <Plus className="w-4 h-4" />}
              {showForm ? "Cancel" : "Assign"}
            </button>
          </div>
        </div>
      </section>

      {showShiftForm && (
        <section className="border border-black/10 p-6 lg:p-10 flex flex-col gap-6">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">Create Shift</p>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Name *</span>
              <input type="text" value={shiftForm.name} onChange={(e) => setShiftForm({ ...shiftForm, name: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" placeholder="Morning Shift" />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Start Time *</span>
              <input type="time" value={shiftForm.startTime} onChange={(e) => setShiftForm({ ...shiftForm, startTime: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">End Time *</span>
              <input type="time" value={shiftForm.endTime} onChange={(e) => setShiftForm({ ...shiftForm, endTime: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Late Threshold (min)</span>
              <input type="number" value={shiftForm.lateThreshold} onChange={(e) => setShiftForm({ ...shiftForm, lateThreshold: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
          </div>
          <div className="flex justify-end">
            <button onClick={handleCreateShift} disabled={shiftSaving} className="bg-black text-white px-6 py-2 text-sm font-mono hover:opacity-80 disabled:opacity-30 cursor-pointer">
              {shiftSaving ? "Creating..." : "Create Shift"}
            </button>
          </div>
        </section>
      )}

      {showForm && (
        <section className="border border-black/10 p-6 lg:p-10 flex flex-col gap-6">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">Assign Schedule</p>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <SelectField label="Employee" value={form.employeeId} onChange={(v) => setForm({ ...form, employeeId: v })} options={employees.map((e) => ({ id: e.id, name: e.fullName }))} required />
            <SelectField label="Shift" value={form.scheduleId} onChange={(v) => setForm({ ...form, scheduleId: v })} options={masters.map((m) => ({ id: m.id, name: `${m.name} (${m.startTime?.slice(0, 5)}-${m.endTime?.slice(0, 5)})` }))} required />
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Date *</span>
              <input type="date" value={form.workDate} onChange={(e) => setForm({ ...form, workDate: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
            <label className="flex flex-col gap-1 justify-end">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Overtime</span>
              <label className="flex items-center gap-2 border border-black/10 px-3 py-2 cursor-pointer">
                <input type="checkbox" checked={form.isOvertime} onChange={(e) => setForm({ ...form, isOvertime: e.target.checked })} className="accent-black" />
                <span className="text-sm font-mono">{form.isOvertime ? "Yes" : "No"}</span>
              </label>
            </label>
          </div>
          <div className="grid grid-cols-1">
            <label className="flex flex-col gap-1">
              <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Note</span>
              <input type="text" value={form.note} onChange={(e) => setForm({ ...form, note: e.target.value })} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </label>
          </div>
          <div className="flex justify-end">
            <button onClick={handleCreate} disabled={saving} className="bg-black text-white px-6 py-2 text-sm font-mono hover:opacity-80 disabled:opacity-30 cursor-pointer">
              {saving ? "Saving..." : "Assign"}
            </button>
          </div>
        </section>
      )}

      {loading ? (
        <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">Loading...</div>
      ) : (
        <>
          <section className="grid grid-cols-7 border-t border-black/10 min-h-[300px]">
            {weekDays.map((day, i) => {
              const dayAssignments = getAssignmentsForDay(day);
              const isToday = day.toDateString() === today.toDateString();
              return (
                <div key={i} className={cn("flex flex-col p-4 border-r border-b border-black/10 min-h-[200px]", isToday ? "bg-black/[0.02]" : "")}>
                  <span className="text-[10px] font-black tracking-swiss opacity-40 mb-1 uppercase">{day.toLocaleDateString("en-US", { weekday: "short" })}</span>
                  <span className="font-heading font-black text-xl mb-2">{day.getDate()}</span>
                  <div className="flex flex-col gap-1 flex-1 overflow-auto">
                    {dayAssignments.map((a) => (
                      <div key={a.id} className="group flex items-center justify-between gap-1 px-1.5 py-1 text-[10px] font-mono bg-black text-white">
                        <span className="truncate">{a.employeeName?.split(" ").pop()}: {a.scheduleName}</span>
                        <button onClick={() => handleDelete(a.id)} className="opacity-0 group-hover:opacity-100 cursor-pointer">
                          <Trash2 className="w-3 h-3" />
                        </button>
                      </div>
                    ))}
                  </div>
                </div>
              );
            })}
          </section>

          <section className="flex flex-col">
            <div className="grid grid-cols-[1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
              <span>Employee</span>
              <span>Date</span>
              <span>Shift</span>
              <span className="text-right">Action</span>
            </div>
            {assignments.slice(0, 20).map((a) => (
              <div key={a.id} className="grid grid-cols-[1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 items-center border-b border-black/5 text-sm font-mono">
                <div>{a.employeeName || a.employeeId?.slice(0, 8)}</div>
                <div className="opacity-60">{a.workDate}</div>
                <div className="flex items-center gap-2">
                  <span className="inline-block w-2 h-2 bg-black" />
                  {a.scheduleName} ({a.startTime?.slice(0, 5)}-{a.endTime?.slice(0, 5)})
                </div>
                <div className="flex justify-end">
                  <button onClick={() => handleDelete(a.id)} className="text-[11px] font-mono underline opacity-40 hover:opacity-100 cursor-pointer">Delete</button>
                </div>
              </div>
            ))}
            {assignments.length === 0 && (
              <div className="px-4 py-8 text-center text-sm font-mono opacity-30">No schedules assigned</div>
            )}
          </section>

          <section className="flex flex-col">
            <p className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 mb-4">Shift Definitions</p>
            <div className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
              <span>Name</span>
              <span>Start</span>
              <span>End</span>
              <span>Late Threshold</span>
              <span className="text-right">Action</span>
            </div>
            {masters.map((s) => (
              <div key={s.id} className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 items-center border-b border-black/5 text-sm font-mono">
                <div>{s.name}</div>
                <div className="opacity-60">{s.startTime?.slice(0, 5)}</div>
                <div className="opacity-60">{s.endTime?.slice(0, 5)}</div>
                <div className="opacity-60">{s.lateThresholdMinutes} min</div>
                <div className="flex justify-end">
                  <button onClick={() => handleDeleteShift(s.id)} className="text-[11px] font-mono underline opacity-40 hover:opacity-100 cursor-pointer">Delete</button>
                </div>
              </div>
            ))}
          </section>
        </>
      )}
    </main>
  );
}

function SelectField({ label, value, onChange, options, required }: {
  label: string; value: string; onChange: (v: string) => void; options: { id: string; name: string }[]; required?: boolean;
}) {
  return (
    <label className="flex flex-col gap-1">
      <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">{label}{required && " *"}</span>
      <select value={value} onChange={(e) => onChange(e.target.value)} className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 bg-white">
        <option value="">--</option>
        {options.map((o) => <option key={o.id} value={o.id}>{o.name}</option>)}
      </select>
    </label>
  );
}
