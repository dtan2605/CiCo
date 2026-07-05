"use client";

import { useState, useEffect, useCallback } from "react";
import { Plus, X, ChevronLeft, ChevronRight, Search } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type UserRecord = {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  roleId: string;
  roleName: string;
  createdAt: string;
  employeeId?: string;
  employeeCode?: string;
  fullName?: string;
  phoneNumber?: string;
  departmentId?: string;
  departmentName?: string;
  positionId?: string;
  positionName?: string;
};

type RoleOption = { id: string; name: string };
type DeptOption = { id: string; name: string };
type PosOption = { id: string; name: string };

type FormData = {
  username: string;
  email: string;
  password: string;
  roleId: string;
  fullName: string;
  employeeCode: string;
  phoneNumber: string;
  departmentId: string;
  positionId: string;
};

const emptyForm: FormData = {
  username: "",
  email: "",
  password: "",
  roleId: "",
  fullName: "",
  employeeCode: "",
  phoneNumber: "",
  departmentId: "",
  positionId: "",
};

export default function AdminUsersPage() {
  const [users, setUsers] = useState<UserRecord[]>([]);
  const [roles, setRoles] = useState<RoleOption[]>([]);
  const [departments, setDepts] = useState<DeptOption[]>([]);
  const [positions, setPositions] = useState<PosOption[]>([]);
  const [page, setPage] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<FormData>(emptyForm);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [deleting, setDeleting] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    try {
      const [userList, roleList, deptList, posList] = await Promise.all([
        api.get<UserRecord[]>(`/admin/users?pageNumber=${page}&pageSize=20&keyword=${keyword}`),
        api.get<RoleOption[]>("/admin/roles"),
        api.get<unknown[]>("/departments?pageNumber=1&pageSize=50"),
        api.get<unknown[]>("/positions?pageNumber=1&pageSize=50"),
      ]);
      setUsers(userList);
      setRoles(roleList);
      setDepts(deptList as DeptOption[]);
      setPositions(posList as PosOption[]);
    } catch { /* ignore */ }
  }, [page, keyword]);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleCreate = async () => {
    setSaving(true);
    setError("");
    try {
      await api.post("/admin/users", {
        ...form,
        departmentId: form.departmentId || null,
        positionId: form.positionId || null,
        employeeCode: form.employeeCode || null,
        phoneNumber: form.phoneNumber || null,
      });
      setShowForm(false);
      setForm(emptyForm);
      fetchData();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Failed to create user");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string) => {
    setDeleting(id);
    try {
      await api.delete(`/admin/users/${id}`);
      fetchData();
    } catch { /* ignore */ }
    finally { setDeleting(null); }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4 mt-auto">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — USER MANAGEMENT
        </p>
        <div className="flex items-center justify-between gap-4">
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            Users
          </h1>
          <div className="flex items-center gap-3">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 opacity-30" />
              <input
                value={keyword}
                onChange={(e) => { setKeyword(e.target.value); setPage(1); }}
                placeholder="Search users..."
                className="border border-black/10 pl-9 pr-3 py-2 text-sm font-mono outline-none focus:border-black/40 transition-colors w-48"
              />
            </div>
            <button
              onClick={() => setShowForm(!showForm)}
              className="bg-black text-white px-4 py-2 text-sm font-mono hover:opacity-80 transition-opacity flex items-center gap-2 cursor-pointer"
            >
              {showForm ? <X className="w-4 h-4" /> : <Plus className="w-4 h-4" />}
              {showForm ? "Cancel" : "New User"}
            </button>
          </div>
        </div>
      </section>

      {error && (
        <div className="border border-black/20 p-4 text-sm font-mono">
          <span className="opacity-40">ERROR: </span>{error}
        </div>
      )}

      {showForm && (
        <section className="border border-black/10 p-6 lg:p-10 flex flex-col gap-6">
          <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">New User</p>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <Field label="Username" value={form.username} onChange={(v) => setForm({ ...form, username: v })} required />
            <Field label="Email" value={form.email} onChange={(v) => setForm({ ...form, email: v })} required />
            <Field label="Password" value={form.password} onChange={(v) => setForm({ ...form, password: v })} type="password" required />
            <Select label="Role" value={form.roleId} onChange={(v) => setForm({ ...form, roleId: v })} options={roles} required />
            <Field label="Full Name" value={form.fullName} onChange={(v) => setForm({ ...form, fullName: v })} required />
            <Field label="Employee Code" value={form.employeeCode} onChange={(v) => setForm({ ...form, employeeCode: v })} />
            <Field label="Phone" value={form.phoneNumber} onChange={(v) => setForm({ ...form, phoneNumber: v })} />
            <Select label="Department" value={form.departmentId} onChange={(v) => setForm({ ...form, departmentId: v })} options={departments} />
            <Select label="Position" value={form.positionId} onChange={(v) => setForm({ ...form, positionId: v })} options={positions} />
          </div>
          <div className="flex justify-end">
            <button
              onClick={handleCreate}
              disabled={saving}
              className="bg-black text-white px-6 py-2 text-sm font-mono hover:opacity-80 transition-opacity disabled:opacity-30 cursor-pointer"
            >
              {saving ? "Creating..." : "Create User"}
            </button>
          </div>
        </section>
      )}

      <section className="flex flex-col">
        <div className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
          <span>Name / Email</span>
          <span>Username</span>
          <span>Role / Dept</span>
          <span>Status</span>
          <span className="text-right">Actions</span>
        </div>
        {users.map((u) => (
          <div
            key={u.id}
            className="grid grid-cols-[1fr_1fr_1fr_1fr_0.5fr] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors"
          >
            <div>
              <div>{u.fullName || "--"}</div>
              <div className="text-[11px] opacity-40">{u.email}</div>
            </div>
            <div className="opacity-60">{u.username}</div>
            <div>
              <div>{u.roleName}</div>
              {u.departmentName && (
                <div className="text-[11px] opacity-40">{u.departmentName}</div>
              )}
            </div>
            <div>
              <span className={cn("inline-block w-2 h-2 rounded-full mr-2", u.isActive ? "bg-black" : "bg-black/20")} />
              {u.isActive ? "Active" : "Inactive"}
            </div>
            <div className="flex justify-end gap-2">
              <button
                onClick={() => handleDelete(u.id)}
                disabled={deleting === u.id}
                className="text-[11px] font-mono underline opacity-40 hover:opacity-100 disabled:opacity-10 cursor-pointer"
              >
                {deleting === u.id ? "..." : "Delete"}
              </button>
            </div>
          </div>
        ))}
        {users.length === 0 && (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-30">No users found</div>
        )}
      </section>

      <div className="flex items-center justify-center gap-4 mt-auto">
        <button
          onClick={() => setPage(Math.max(1, page - 1))}
          disabled={page === 1}
          className="border border-black/10 px-3 py-1 text-sm font-mono hover:opacity-60 disabled:opacity-20 cursor-pointer"
        >
          <ChevronLeft className="w-4 h-4" />
        </button>
        <span className="text-sm font-mono opacity-40">Page {page}</span>
        <button
          onClick={() => setPage(page + 1)}
          disabled={users.length < 20}
          className="border border-black/10 px-3 py-1 text-sm font-mono hover:opacity-60 disabled:opacity-20 cursor-pointer"
        >
          <ChevronRight className="w-4 h-4" />
        </button>
      </div>
    </main>
  );
}

function Field({
  label, value, onChange, type, required,
}: {
  label: string; value: string; onChange: (v: string) => void; type?: string; required?: boolean;
}) {
  return (
    <label className="flex flex-col gap-1">
      <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">
        {label}{required && " *"}
      </span>
      <input
        type={type || "text"}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 transition-colors"
      />
    </label>
  );
}

function Select({
  label, value, onChange, options, required,
}: {
  label: string; value: string; onChange: (v: string) => void; options: { id: string; name: string }[]; required?: boolean;
}) {
  return (
    <label className="flex flex-col gap-1">
      <span className="text-[10px] font-heading font-black tracking-swiss uppercase opacity-40">
        {label}{required && " *"}
      </span>
      <select
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40 transition-colors bg-white"
      >
        <option value="">--</option>
        {options.map((o) => (
          <option key={o.id} value={o.id}>{o.name}</option>
        ))}
      </select>
    </label>
  );
}
