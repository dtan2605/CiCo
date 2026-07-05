"use client";

import { useState } from "react";
import { Send } from "lucide-react";
import { api } from "@/lib/api-client";

const NOTIF_TYPES = [
  { value: 1, label: "Info" },
  { value: 2, label: "Warning" },
  { value: 3, label: "System" },
];

export default function AdminNotificationsPage() {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [type, setType] = useState(1);
  const [sending, setSending] = useState(false);
  const [done, setDone] = useState(false);
  const [error, setError] = useState("");

  const handleSend = async () => {
    if (!title || !content) return;
    setSending(true);
    setError("");
    setDone(false);
    try {
      await api.post("/admin/notifications/broadcast", {
        title,
        content,
        type,
      });
      setDone(true);
      setTitle("");
      setContent("");
      setType(1);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Failed to send");
    } finally {
      setSending(false);
    }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — NOTIFICATIONS
        </p>
        <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
          Broadcast Notification
        </h1>
      </section>

      <section className="border border-black/10 p-6 lg:p-10 flex flex-col gap-6 max-w-2xl">
        <div className="flex flex-col gap-1">
          <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Title *</span>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40"
            placeholder="Notification title"
          />
        </div>
        <div className="flex flex-col gap-1">
          <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Content *</span>
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            rows={4}
            className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 resize-none"
            placeholder="Notification content..."
          />
        </div>
        <div className="flex flex-col gap-1">
          <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">Type</span>
          <select
            value={type}
            onChange={(e) => setType(Number(e.target.value))}
            className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 bg-white"
          >
            {NOTIF_TYPES.map((t) => (
              <option key={t.value} value={t.value}>{t.label}</option>
            ))}
          </select>
        </div>

        {error && (
          <div className="border border-black/20 p-3 text-sm font-mono">
            <span className="opacity-40">ERROR: </span>{error}
          </div>
        )}
        {done && (
          <div className="border border-black p-3 text-sm font-mono">
            Notification sent to all employees
          </div>
        )}

        <div className="flex justify-end">
          <button
            onClick={handleSend}
            disabled={sending || !title || !content}
            className="bg-black text-white px-6 py-2 text-sm font-mono hover:opacity-80 disabled:opacity-30 flex items-center gap-2 cursor-pointer"
          >
            <Send className="w-4 h-4" />
            {sending ? "Sending..." : "Broadcast"}
          </button>
        </div>
      </section>
    </main>
  );
}
