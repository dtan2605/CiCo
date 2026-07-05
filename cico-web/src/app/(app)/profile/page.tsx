"use client";

import { useState, useEffect, useRef } from "react";
import {
  User,
  Mail,
  Briefcase,
  MapPin,
  Shield,
  Camera,
  Phone,
  Home,
  Send,
  Clock,
} from "lucide-react";
import { api, API_ORIGIN } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type EmployeeProfile = {
  id: string;
  employeeCode: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  gender: number;
  avatarUrl: string | null;
  departmentId: string;
  positionId: string;
  isActive: boolean;
  departmentName?: string;
  positionName?: string;
  address?: string;
};

type UpdateRequest = {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: string;
  status: string;
  createdAt: string;
};

export default function ProfilePage() {
  const [profile, setProfile] =
    useState<EmployeeProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [requests, setRequests] = useState<UpdateRequest[]>([]);
  const [showRequests, setShowRequests] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    address: "",
  });

  useEffect(() => {
    const userId = localStorage.getItem("userId");
    if (!userId) return;
    setLoading(true);
    api.get<EmployeeProfile>(`/Employees/by-user/${userId}`)
      .then((p) => {
        setProfile(p);
        setForm({
          fullName: p.fullName,
          email: p.email,
          phoneNumber: p.phoneNumber,
          address: p.address ?? "",
        });
      })
      .catch(() => {})
      .finally(() => setLoading(false));

    api.get<UpdateRequest[]>("/profile-update-requests/my")
      .then(setRequests)
      .catch(() => {});
  }, []);

  const handleSubmitRequest = async () => {
    setSubmitting(true);
    try {
      await api.post("/profile-update-requests", form);
      setSubmitted(true);
      setEditing(false);
      const updated = await api.get<UpdateRequest[]>("/profile-update-requests/my");
      setRequests(updated);
    } catch { /* ignore */ }
    finally { setSubmitting(false); }
  };

  const handleCancel = () => {
    if (!profile) return;
    setForm({
      fullName: profile.fullName,
      email: profile.email,
      phoneNumber: profile.phoneNumber,
      address: profile.address ?? "",
    });
    setEditing(false);
  };

  const handleAvatarChange = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploading(true);
    try {
      const reader = new FileReader();
      reader.onload = async () => {
        const base64 = reader.result as string;
        const { avatarUrl } = await api.put<{ avatarUrl: string }>(
          "/profile/avatar",
          { imageBase64: base64 }
        );
        setProfile((prev) =>
          prev ? { ...prev, avatarUrl } : prev
        );
      };
      reader.readAsDataURL(file);
    } catch { /* ignore */ }
    finally { setUploading(false); }
  };

  const latestPending = requests.find(
    (r) => r.status === "Pending"
  );

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-16 lg:gap-24">
      <header className="flex items-center justify-between">
        <div className="flex flex-col gap-2">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
            Personal Account
          </p>
          <h1 className="font-heading font-black text-6xl tracking-tight">
            EMPLOYEE PROFILE
          </h1>
        </div>
        {!editing && !submitted && !loading && profile && (
          <button
            onClick={() => setEditing(true)}
            className="swiss-button text-xs"
          >
            Edit Information
          </button>
        )}
        {submitted && (
          <span className="text-[10px] font-black tracking-swiss text-green-600 uppercase">
            Request submitted
          </span>
        )}
      </header>

      {loading && (
        <div className="py-20 text-center text-[10px] font-black tracking-swiss opacity-20 uppercase">
          Loading profile...
        </div>
      )}

      {profile && (
        <>
          <div className="flex flex-col xl:flex-row gap-20">
            <div className="flex flex-col gap-8 shrink-0">
              <div className="w-64 h-64 bg-surface border border-border-muted relative group overflow-hidden">
                {profile.avatarUrl ? (
                  <img
                    src={`${API_ORIGIN}${profile.avatarUrl}`}
                    alt={profile.fullName}
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <div className="absolute inset-0 flex items-center justify-center opacity-10">
                    <User className="w-24 h-24" />
                  </div>
                )}
                <button
                  onClick={() => fileInputRef.current?.click()}
                  disabled={uploading}
                  className="absolute bottom-4 right-4 bg-black text-white p-3 hover:scale-110 transition-transform disabled:opacity-40"
                >
                  <Camera className="w-5 h-5" />
                </button>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  onChange={handleAvatarChange}
                  className="hidden"
                />
              </div>
              {uploading && (
                <span className="text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  Uploading...
                </span>
              )}
              <div className="flex flex-col gap-1">
                <h2 className="font-heading font-black text-2xl tracking-tight">
                  {profile.fullName.toUpperCase()}
                </h2>
                <span className="text-[10px] font-black tracking-swiss opacity-40">
                  {profile.employeeCode}
                </span>
              </div>
            </div>

            <div className="flex-1 grid grid-cols-1 md:grid-cols-2 gap-y-12 gap-x-20 border-t border-border-muted pt-12">
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <User className="w-3 h-3" /> Full Name
                </label>
                {editing ? (
                  <input
                    value={form.fullName}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, fullName: e.target.value }))
                    }
                    className="font-heading font-black text-lg tracking-tight border-b-2 border-black pb-4 outline-none bg-transparent"
                  />
                ) : (
                  <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                    {profile.fullName}
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <Mail className="w-3 h-3" /> Email Address
                </label>
                {editing ? (
                  <input
                    value={form.email}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, email: e.target.value }))
                    }
                    className="font-heading font-black text-lg tracking-tight border-b-2 border-black pb-4 outline-none bg-transparent"
                  />
                ) : (
                  <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                    {profile.email}
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <Briefcase className="w-3 h-3" /> Position
                </label>
                <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                  {profile.positionName ?? profile.positionId}
                </span>
              </div>
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <Shield className="w-3 h-3" /> Department
                </label>
                <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                  {profile.departmentName ?? profile.departmentId}
                </span>
              </div>
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <Phone className="w-3 h-3" /> Phone
                </label>
                {editing ? (
                  <input
                    value={form.phoneNumber}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, phoneNumber: e.target.value }))
                    }
                    className="font-heading font-black text-lg tracking-tight border-b-2 border-black pb-4 outline-none bg-transparent"
                  />
                ) : (
                  <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                    {profile.phoneNumber || "—"}
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-4">
                <label className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 uppercase">
                  <Home className="w-3 h-3" /> Address
                </label>
                {editing ? (
                  <input
                    value={form.address}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, address: e.target.value }))
                    }
                    className="font-heading font-black text-lg tracking-tight border-b-2 border-black pb-4 outline-none bg-transparent"
                  />
                ) : (
                  <span className="font-heading font-black text-lg tracking-tight border-b border-border-muted pb-4">
                    {profile.address || "—"}
                  </span>
                )}
              </div>
            </div>
          </div>

          {editing && (
            <div className="flex gap-4 justify-end border-t border-border-muted pt-8">
              <button
                onClick={handleCancel}
                disabled={submitting}
                className="swiss-button-outline text-xs flex items-center gap-2"
              >
                Cancel Edit
              </button>
              <button
                onClick={handleSubmitRequest}
                disabled={submitting}
                className="swiss-button text-xs flex items-center gap-2"
              >
                <Send className="w-4 h-4" />
                {submitting ? "Submitting..." : "Submit for Approval"}
              </button>
            </div>
          )}

          {latestPending && (
            <div className="border border-yellow-400/30 bg-yellow-50 px-6 py-4 text-sm font-mono flex items-center gap-3">
              <Clock className="w-4 h-4 text-yellow-600" />
              <span>
                You have a pending profile update request submitted{" "}
                {new Date(latestPending.createdAt).toLocaleDateString()}.
              </span>
            </div>
          )}

          {!editing && (
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-12 mt-12 pt-12 border-t border-border-muted">
              <div className="flex flex-col gap-2">
                <h3 className="font-heading font-black text-[10px] tracking-swiss opacity-40 uppercase">
                  Employment Status
                </h3>
                <p className="font-heading font-black text-2xl">
                  {profile.isActive ? "ACTIVE" : "INACTIVE"}
                </p>
              </div>
              <div className="flex flex-col gap-2">
                <h3 className="font-heading font-black text-[10px] tracking-swiss opacity-40 uppercase">
                  Employee ID
                </h3>
                <p className="font-heading font-black text-2xl">
                  {profile.employeeCode}
                </p>
              </div>
              <div className="flex flex-col gap-6">
                <button
                  onClick={() => { setEditing(true); setSubmitted(false); }}
                  className="swiss-button w-full flex items-center justify-between text-xs"
                >
                  <span>Edit Information</span>
                  <Shield className="w-5 h-5" />
                </button>
              </div>
            </div>
          )}

          {requests.length > 0 && !editing && (
            <div className="border-t border-border-muted pt-12">
              <button
                onClick={() => setShowRequests(!showRequests)}
                className="flex items-center gap-2 text-[10px] font-black tracking-swiss opacity-40 hover:opacity-100 transition-opacity uppercase mb-6"
              >
                <Clock className="w-3 h-3" />
                Profile Update History ({requests.length})
              </button>
              {showRequests && (
                <div className="flex flex-col">
                  <div className="grid grid-cols-[1fr_1fr_1fr_1fr_auto] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
                    <span>Date</span>
                    <span>Name</span>
                    <span>Email</span>
                    <span>Phone</span>
                    <span>Status</span>
                  </div>
                  {requests.map((r) => (
                    <div
                      key={r.id}
                      className="grid grid-cols-[1fr_1fr_1fr_1fr_auto] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors"
                    >
                      <div className="opacity-60">
                        {new Date(r.createdAt).toLocaleDateString()}
                      </div>
                      <div>{r.fullName}</div>
                      <div className="opacity-60 text-[13px]">{r.email}</div>
                      <div className="opacity-60">{r.phoneNumber}</div>
                      <span
                        className={cn(
                          "text-[10px] font-black tracking-swiss uppercase",
                          r.status === "Approved"
                            ? "text-green-600"
                            : r.status === "Rejected"
                            ? "text-red-600"
                            : "text-yellow-600"
                        )}
                      >
                        {r.status}
                      </span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </>
      )}
    </main>
  );
}
