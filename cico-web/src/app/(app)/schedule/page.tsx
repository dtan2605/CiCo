"use client";

import { useState, useEffect, useCallback } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import Link from "next/link";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type ScheduleItem = {
  id: string;
  employeeId: string;
  scheduleId: string;
  workDate: string;
  isOvertime: boolean;
  note: string;
  startTime: string;
  endTime: string;
  scheduleName: string;
};

type ScheduleMaster = {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  lateThresholdMinutes: number;
  isActive: boolean;
};

export default function SchedulePage() {
  const [employeeId, setEmployeeId] = useState<string | null>(null);
  const [schedules, setSchedules] = useState<ScheduleItem[]>([]);
  const [masterSchedules, setMasterSchedules] = useState<
    ScheduleMaster[]
  >([]);
  const [weekOffset, setWeekOffset] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const uid = localStorage.getItem("userId");
    if (!uid) return;
    api.get<{ id: string }>(`/Employees/by-user/${uid}`)
      .then((emp) => setEmployeeId(emp.id))
      .catch(() => setLoading(false));
  }, []);

  const getWeekDays = useCallback(() => {
    const today = new Date();
    const start = new Date(today);
    start.setDate(
      today.getDate() +
        weekOffset * 7 -
        today.getDay() +
        1
    );
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(start);
      d.setDate(start.getDate() + i);
      return d;
    });
  }, [weekOffset]);

  useEffect(() => {
    if (!employeeId) return;
    setLoading(true);
    Promise.all([
      api
        .get<ScheduleItem[]>(
          `/employeeschedule?employeeId=${employeeId}&pageSize=50`
        )
        .catch(() => [] as ScheduleItem[]),
      api
        .get<ScheduleMaster[]>("/Schedules")
        .catch(() => [] as ScheduleMaster[]),
    ])
      .then(([empSchedules, masters]) => {
        setSchedules(empSchedules);
        setMasterSchedules(masters);
      })
      .finally(() => setLoading(false));
  }, [employeeId]);

  const days = getWeekDays();
  const year = days[0].getFullYear();
  const month = days[0].toLocaleString("en-US", {
    month: "long",
  });
  const weekNum = Math.ceil(
    (days[0].getTime() -
      new Date(year, 0, 1).getTime()) /
      604800000
  );

  const getShiftForDay = (date: Date) => {
    const dateStr = date.toISOString().split("T")[0];
    const emp = schedules.find(
      (s) => s.workDate === dateStr
    );
    if (!emp) return null;
    return {
      start: emp.startTime,
      end: emp.endTime,
      name: emp.scheduleName,
    };
  };

  const totalAssigned = schedules
    .filter((s) => {
      const d = new Date(s.workDate);
      const weekStart = days[0];
      const weekEnd = days[6];
      return d >= weekStart && d <= weekEnd;
    })
    .reduce((sum, s) => {
      if (s.isOvertime) return sum + 0;
      const [h] = empStartEnd(s);
      return sum + h;
    }, 0);

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-16 lg:gap-24">
      <header className="flex flex-col lg:flex-row lg:items-end justify-between gap-8">
        <div className="flex flex-col gap-2">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            Shift Management
          </p>
          <h1 className="font-heading font-black text-6xl tracking-tight">
            WORK SCHEDULE
          </h1>
        </div>
        <div className="flex items-center gap-6 border border-border-muted p-4">
          <button
            onClick={() => setWeekOffset((w) => w - 1)}
            className="opacity-40 hover:opacity-100"
          >
            <ChevronLeft className="w-5 h-5" />
          </button>
          <span className="font-heading font-black text-xs tracking-swiss uppercase">
            {month} {year} — WEEK {weekNum}
          </span>
          <button
            onClick={() => setWeekOffset((w) => w + 1)}
            className="opacity-40 hover:opacity-100"
          >
            <ChevronRight className="w-5 h-5" />
          </button>
        </div>
      </header>

      {loading && (
        <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
          Loading schedule...
        </div>
      )}

      {!loading && (
        <>
          <section className="grid grid-cols-1 md:grid-cols-7 border-t border-border-muted min-h-[400px]">
            {days.map((day, i) => {
              const shift = getShiftForDay(day);
              const today = new Date();
              const isToday =
                day.toDateString() === today.toDateString();
              return (
                <div
                  key={i}
                  className={cn(
                    "flex flex-col p-8 border-r border-b border-border-muted group hover:bg-surface transition-all",
                    isToday ? "bg-surface" : ""
                  )}
                >
                  <span className="text-[10px] font-black tracking-swiss opacity-40 mb-2">
                    {day
                      .toLocaleString("en-US", {
                        weekday: "long",
                      })
                      .toUpperCase()}
                  </span>
                  <span className="font-heading font-black text-5xl mb-auto">
                    {day.getDate()}
                  </span>
                  <div className="mt-auto pt-8 flex flex-col gap-1">
                    {shift ? (
                      <>
                        <span className="text-[10px] font-black tracking-swiss opacity-40 uppercase">
                          {shift.name}
                        </span>
                        <span className="font-heading font-black text-lg tracking-tight leading-tight">
                          {shift.start?.slice(0, 5) ?? "—"}
                          <span className="opacity-30 mx-1">—</span>
                          {shift.end?.slice(0, 5) ?? "—"}
                        </span>
                      </>
                    ) : (
                      <span className="font-heading font-black text-lg opacity-20 tracking-tight">
                        OFF
                      </span>
                    )}
                  </div>
                </div>
              );
            })}
          </section>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
            <div className="p-12 border border-black flex flex-col gap-6">
              <h3 className="font-heading font-black text-xs tracking-swiss uppercase">
                Shift Statistics
              </h3>
              <div className="grid grid-cols-3 gap-8">
                <div className="flex flex-col gap-1">
                  <span className="text-[10px] opacity-40 uppercase font-black tracking-swiss">
                    Weekly Target
                  </span>
                  <span className="font-heading font-black text-2xl">
                    40H
                  </span>
                </div>
                <div className="flex flex-col gap-1">
                  <span className="text-[10px] opacity-40 uppercase font-black tracking-swiss">
                    Total Assigned
                  </span>
                  <span className="font-heading font-black text-2xl">
                    {totalAssigned}H
                  </span>
                </div>
                <div className="flex flex-col gap-1">
                  <span className="text-[10px] opacity-40 uppercase font-black tracking-swiss">
                    Overtime Est.
                  </span>
                  <span className="font-heading font-black text-2xl underline underline-offset-4">
                    {Math.max(0, totalAssigned - 40)}H
                  </span>
                </div>
              </div>
            </div>
            <div className="p-12 bg-surface flex items-center justify-between gap-8 border border-border-muted">
              <div className="flex flex-col gap-2">
                <h3 className="font-heading font-black text-xs tracking-swiss uppercase">
                  Request Change
                </h3>
                <p className="text-xs opacity-60 leading-relaxed max-w-xs">
                  Need to adjust your shift? Submit a request
                  for manager approval.
                </p>
              </div>
              <Link href="/requests" className="swiss-button text-xs !px-8 !py-4 shrink-0 inline-flex items-center justify-center">
                Submit Request
              </Link>
            </div>
          </div>
        </>
      )}
    </main>
  );
}

function empStartEnd(s: ScheduleItem): [number, string, string] {
  const start = s.startTime
    ? parseInt(s.startTime.split(":")[0] ?? "0", 10)
    : 0;
  const end = s.endTime
    ? parseInt(s.endTime.split(":")[0] ?? "0", 10)
    : 0;
  return [end - start, s.startTime, s.endTime];
}
