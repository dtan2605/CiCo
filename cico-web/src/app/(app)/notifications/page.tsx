"use client";

import { useState, useEffect, useCallback } from "react";
import {
  Bell,
  CheckCircle,
  AlertTriangle,
  Info,
  Clock,
} from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type NotificationItem = {
  id: string;
  title: string;
  content: string;
  type: number;
  isRead: boolean;
  employeeId: string;
  createdAt: string;
};

const typeMeta: Record<
  number,
  { label: string; icon: typeof Bell }
> = {
  1: { label: "INFO", icon: Info },
  2: { label: "WARNING", icon: AlertTriangle },
  3: { label: "SYSTEM", icon: Clock },
};

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<
    NotificationItem[]
  >([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const data = await api.get<NotificationItem[]>(
        `/notification?pageNumber=${page}&pageSize=50`
      );
      setNotifications(data ?? []);
    } catch {
      /* ignore */
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const markAsRead = async (id: string) => {
    try {
      await api.put(`/notification/${id}/read`);
      setNotifications((prev) =>
        prev.map((n) =>
          n.id === id ? { ...n, isRead: true } : n
        )
      );
    } catch {
      /* ignore */
    }
  };

  const markAllAsRead = async () => {
    const userId = localStorage.getItem("userId");
    if (!userId) return;
    try {
      await api.put("/notification/read-all", {
        userId,
      });
      setNotifications((prev) =>
        prev.map((n) => ({ ...n, isRead: true }))
      );
    } catch {
      /* ignore */
    }
  };

  const timeAgo = (dateStr: string) => {
    const diff =
      Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diff / 60000);
    if (mins < 60) return `${mins} MINUTES AGO`;
    const hours = Math.floor(mins / 60);
    if (hours < 24) return `${hours} HOURS AGO`;
    const days = Math.floor(hours / 24);
    return `${days} DAYS AGO`;
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-16 lg:gap-24">
      <header className="flex flex-col lg:flex-row lg:items-end justify-between gap-8">
        <div className="flex flex-col gap-2">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            Communications Center
          </p>
          <h1 className="font-heading font-black text-6xl tracking-tight">
            NOTIFICATIONS
          </h1>
        </div>
        <button
          onClick={markAllAsRead}
          className="text-[10px] font-black tracking-swiss uppercase border border-border-muted px-6 py-3 hover:bg-black hover:text-white transition-all"
        >
          Mark All As Read
        </button>
      </header>

      <section className="flex flex-col border-t border-border-muted">
        {loading && (
          <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
            Loading notifications...
          </div>
        )}
        {!loading && notifications.length === 0 && (
          <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
            No notifications
          </div>
        )}
        {notifications.map((notif) => {
          const meta = typeMeta[notif.type] ?? {
            label: "INFO",
            icon: Bell,
          };
          const Icon = meta.icon;
          return (
            <div
              key={notif.id}
              className={cn(
                "flex flex-col lg:flex-row gap-8 py-12 px-4 border-b border-border-muted group transition-all",
                notif.isRead
                  ? "opacity-40 grayscale"
                  : "bg-white"
              )}
            >
              <div className="shrink-0">
                <div
                  className={cn(
                    "w-12 h-12 flex items-center justify-center border",
                    notif.isRead
                      ? "border-border-muted"
                      : "border-black bg-black text-white"
                  )}
                >
                  <Icon className="w-5 h-5" />
                </div>
              </div>
              <div className="flex-1 flex flex-col gap-4">
                <header className="flex items-center justify-between">
                  <div className="flex items-center gap-4">
                    <span className="text-[10px] font-black tracking-swiss uppercase">
                      {meta.label}
                    </span>
                    <span className="h-1 w-1 bg-black rounded-full" />
                    <h3 className="font-heading font-black text-lg tracking-tight">
                      {notif.title}
                    </h3>
                  </div>
                  <span className="text-[10px] font-black tracking-swiss opacity-40">
                    {timeAgo(notif.createdAt)}
                  </span>
                </header>
                <p className="text-sm leading-relaxed max-w-2xl font-medium">
                  {notif.content}
                </p>
              </div>
              <div className="flex items-center">
                {!notif.isRead && (
                  <button
                    onClick={() => markAsRead(notif.id)}
                    className="text-[10px] font-black tracking-swiss uppercase border border-black px-4 py-2 hover:bg-black hover:text-white transition-all"
                  >
                    Dismiss
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </section>

      <footer className="py-12 flex justify-center">
        <button
          onClick={() => setPage((p) => p + 1)}
          className="text-[10px] font-black tracking-swiss uppercase opacity-20 hover:opacity-100 transition-opacity"
        >
          Load More
        </button>
      </footer>
    </main>
  );
}
