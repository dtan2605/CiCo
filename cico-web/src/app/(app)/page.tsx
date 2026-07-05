"use client";

import { useState, useEffect, useCallback } from "react";
import { ChevronRight } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn, localDate, localISOString } from "@/lib/utils";

type AttendanceRecord = {
  id: string;
  attendanceDate: string;
  checkInTime: string | null;
  checkOutTime: string | null;
  lateMinutes: number;
  status: string;
  method: number;
  employeeId: string;
  employeeScheduleId?: string;
};

type DashboardData = {
  totalEmployees: number;
  presentToday: number;
  absentToday: number;
};

type NotificationItem = {
  id: string;
  title: string;
  content: string;
  type: number;
  isRead: boolean;
  employeeId: string;
  createdAt: string;
};

export default function DashboardPage() {
  const [time, setTime] = useState(new Date());
  const [userId, setUserId] = useState<string | null>(null);
  const [employeeId, setEmployeeId] = useState<string | null>(null);
  const [attendance, setAttendance] = useState<AttendanceRecord | null>(null);
  const [todayLogs, setTodayLogs] = useState<AttendanceRecord[]>([]);
  const [dashboard, setDashboard] = useState<DashboardData | null>(null);
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);
  const [checking, setChecking] = useState(false);

  useEffect(() => {
    const uid = localStorage.getItem("userId");
    setUserId(uid);
    if (uid) {
      api.get<{ id: string }>(`/Employees/by-user/${uid}`)
        .then((emp) => setEmployeeId(emp.id))
        .catch(() => {});
    }
  }, []);

  useEffect(() => {
    const timer = setInterval(
      () => setTime(new Date()),
      1000
    );
    return () => clearInterval(timer);
  }, []);

  const fetchData = useCallback(async () => {
    if (!userId || !employeeId) return;
    const today = localDate();

    try {
      const [attendanceList, dash, unread] =
        await Promise.all([
          api.get<AttendanceRecord[]>(
            `/attendance/by-employee/${employeeId}`
          ),
          api.get<DashboardData>(
            `/report/dashboard?date=${today}`
          ),
          api.get<NotificationItem[]>(
            `/notification/unread`
          ),
        ]);

      setTodayLogs(attendanceList.slice(0, 10));
      setDashboard(dash);
      setNotifications(
        (unread ?? []).slice(0, 4)
      );

      const todayRecord = attendanceList.find(
        (a) =>
          a.attendanceDate === today
      );
      setAttendance(todayRecord ?? null);
    } catch {
      /* ignore */
    }
  }, [userId, employeeId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleCheck = async () => {
    if (!userId || !employeeId) return;
    setChecking(true);
    try {
      if (attendance?.checkInTime && !attendance.checkOutTime) {
        await api.put("/attendance/check-out", {
          employeeId,
          checkOutTime: localISOString(),
        });
      } else {
        await api.post("/attendance/check-in", {
          employeeId,
          checkInTime: localISOString(),
          method: 3,
        });
      }
      await fetchData();
    } catch {
      /* ignore */
    } finally {
      setChecking(false);
    }
  };

  const status =
    attendance?.checkInTime && !attendance.checkOutTime
      ? "IN"
      : "OUT";

  const formatTime = (date: Date) =>
    date.toLocaleTimeString("en-US", {
      hour12: false,
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    });

  const fmt = (dt: string | null) =>
    dt
      ? new Date(dt).toLocaleTimeString("en-US", {
          hour12: false,
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        })
      : "--";

  return (
    <>
      <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-16 lg:gap-32">
        <section className="flex flex-col gap-4 mt-auto">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            CURRENT STATUS: SIGNED {status}
          </p>
          <div className="flex flex-col lg:flex-row lg:items-end justify-between gap-8 lg:gap-12">
            <h1 className="font-heading font-black text-[clamp(4rem,15vw,180px)] leading-[0.75] tracking-tighter">
              {formatTime(time)}
            </h1>
            <button
              onClick={handleCheck}
              disabled={checking}
              className="swiss-button flex items-center gap-6 mb-4 min-w-[280px] justify-between"
            >
              <span>
                {checking
                  ? "Processing..."
                  : status === "IN"
                  ? "EXECUTE CHECK OUT"
                  : "EXECUTE CHECK IN"}
              </span>
              <ChevronRight className="w-6 h-6" />
            </button>
          </div>
        </section>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-16 lg:gap-32">
          <section className="flex flex-col gap-10">
            <h2 className="font-heading font-black text-[10px] tracking-swiss opacity-40 uppercase">
              Daily Log
            </h2>
            <div className="flex flex-col">
              {todayLogs.length === 0 && (
                <div className="hairline-b py-6 text-[10px] opacity-20 font-black tracking-swiss uppercase">
                  No records today
                </div>
              )}
              {todayLogs.map((row) => (
                <div
                  key={row.id}
                  className="hairline-b py-6 flex items-center justify-between"
                >
                  <div className="flex items-center gap-6">
                    <div
                      className={cn(
                        "w-1.5 h-1.5 rounded-none",
                        row.status === "Late"
                          ? "bg-red-500"
                          : "bg-black"
                      )}
                    />
                    <span className="font-medium tracking-tight uppercase text-xs">
                      {row.attendanceDate} — {row.status}
                    </span>
                  </div>
                  <div className="flex gap-4 font-mono text-[10px] opacity-40 tracking-widest">
                    <span>{fmt(row.checkInTime)}</span>
                    <span>{fmt(row.checkOutTime)}</span>
                  </div>
                </div>
              ))}
            </div>
          </section>

          <section className="flex flex-col gap-10">
            <h2 className="font-heading font-black text-[10px] tracking-swiss opacity-40 uppercase">
              Today&apos;s Overview
            </h2>
            {dashboard && (
              <div className="flex flex-col gap-6">
                <div className="grid grid-cols-3 gap-6">
                  {[
                    {
                      label: "Total",
                      value: dashboard.totalEmployees,
                    },
                    {
                      label: "Present",
                      value: dashboard.presentToday,
                    },
                    {
                      label: "Absent",
                      value: dashboard.absentToday,
                    },
                  ].map((item) => (
                    <div
                      key={item.label}
                      className="flex flex-col gap-2 p-6 bg-surface border border-border-muted"
                    >
                      <span className="text-[10px] font-black tracking-swiss opacity-40 uppercase">
                        {item.label}
                      </span>
                      <span className="font-heading font-black text-3xl">
                        {item.value}
                      </span>
                    </div>
                  ))}
                </div>
                <div className="h-4 w-full bg-surface flex">
                  {dashboard.totalEmployees > 0 && (
                    <>
                      <div
                        className="h-full bg-black transition-all"
                        style={{
                          width: `${
                            (dashboard.presentToday /
                              dashboard.totalEmployees) *
                            100
                          }%`,
                        }}
                      />
                      <div
                        className="h-full bg-neutral-300 transition-all"
                        style={{
                          width: `${
                            (dashboard.absentToday /
                              dashboard.totalEmployees) *
                            100
                          }%`,
                        }}
                      />
                    </>
                  )}
                </div>
                <div className="flex justify-between text-[10px] uppercase tracking-swiss font-black opacity-30 px-1">
                  <span>Present ({dashboard.presentToday})</span>
                  <span>Absent ({dashboard.absentToday})</span>
                </div>
              </div>
            )}
          </section>
        </div>
      </main>

      <aside className="w-96 border-l border-border-muted p-20 hidden 2xl:flex flex-col gap-16 bg-surface shrink-0">
        <h2 className="font-heading font-black text-[10px] tracking-swiss opacity-40 hairline-b pb-4 uppercase">
          Notifications
        </h2>
        <div className="flex flex-col gap-12">
          {notifications.length === 0 && (
            <span className="text-[10px] opacity-20 font-black tracking-swiss uppercase">
              No notifications
            </span>
          )}
          {notifications.map((alert) => (
            <div key={alert.id} className="flex flex-col gap-2">
              <span
                className={cn(
                  "font-heading font-black text-[10px] tracking-swiss",
                  !alert.isRead
                    ? "text-black"
                    : "opacity-30"
                )}
              >
                {alert.title}
              </span>
              <span className="text-[10px] opacity-40 font-medium tracking-tight">
                {alert.content}
              </span>
            </div>
          ))}
        </div>
      </aside>
    </>
  );
}
