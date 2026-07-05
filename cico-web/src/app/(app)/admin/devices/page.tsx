"use client";

import { useState, useEffect, useCallback } from "react";
import { Plus, X, Power, RefreshCw, Wifi, WifiOff } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type Device = {
  id: string;
  deviceCode: string;
  name: string;
  location: string;
  ipAddress: string;
  port: number;
  serialNumber: string;
  manufacturer: string;
  firmwareVersion: string;
  lastSyncTime: string | null;
  status: string;
  isActive: boolean;
};

type DevicePage = {
  items: Device[];
  total: number;
  page: number;
  size: number;
};

export default function AdminDevicesPage() {
  const [data, setData] = useState<DevicePage | null>(null);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [syncing, setSyncing] = useState(false);
  const [testingId, setTestingId] = useState<string | null>(null);

  const emptyForm = {
    deviceCode: "",
    name: "",
    location: "",
    ipAddress: "",
    port: 80,
    serialNumber: "",
    manufacturer: "Hikvision",
    firmwareVersion: "",
    username: "",
    password: "",
  };

  const [form, setForm] = useState(emptyForm);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const result = await api.get<DevicePage>("/devices");
      setData(result);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleSave = async () => {
    try {
      if (editId) {
        await api.put(`/devices/${editId}`, { ...form, isActive: true });
      } else {
        await api.post("/devices", form);
      }
      setShowForm(false);
      setEditId(null);
      setForm(emptyForm);
      fetchData();
    } catch { /* ignore */ }
  };

  const handleEdit = (device: Device) => {
    setEditId(device.id);
    setForm({
      deviceCode: device.deviceCode,
      name: device.name,
      location: device.location,
      ipAddress: device.ipAddress,
      port: device.port,
      serialNumber: device.serialNumber,
      manufacturer: device.manufacturer,
      firmwareVersion: device.firmwareVersion,
      username: "",
      password: "",
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Delete this device?")) return;
    try {
      await api.delete(`/devices/${id}`);
      fetchData();
    } catch { /* ignore */ }
  };

  const handleTestConnection = async (id: string) => {
    setTestingId(id);
    try {
      const result = await api.post<{ success: boolean; message: string; status: string }>(
        `/devices/${id}/test-connection`
      );
      alert(`${result.success ? "OK" : "FAIL"}: ${result.message}`);
      fetchData();
    } catch { /* ignore */ }
    finally { setTestingId(null); }
  };

  const handleSyncAll = async () => {
    setSyncing(true);
    try {
      const result = await api.post<{ synced: number; details: { id: string; name: string; recordsFound: number; processed: number; error: string | null }[] }>(
        "/devices/sync-all"
      );
      const msg = result.details
        .map((d) => `${d.name}: ${d.processed}/${d.recordsFound} records${d.error ? ` (${d.error})` : ""}`)
        .join("\n");
      alert(`Synced ${result.synced} devices:\n${msg}`);
      fetchData();
    } catch { /* ignore */ }
    finally { setSyncing(false); }
  };

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          ADMIN — DEVICES
        </p>
        <div className="flex items-center justify-between">
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            Attendance Devices
          </h1>
          <div className="flex gap-3">
            <button
              onClick={handleSyncAll}
              disabled={syncing}
              className="swiss-button-outline text-xs flex items-center gap-2"
            >
              <RefreshCw className={cn("w-4 h-4", syncing && "animate-spin")} />
              Sync All
            </button>
            <button
              onClick={() => { setShowForm(true); setEditId(null); setForm(emptyForm); }}
              className="swiss-button text-xs flex items-center gap-2"
            >
              <Plus className="w-4 h-4" /> Add Device
            </button>
          </div>
        </div>
      </section>

      {showForm && (
        <section className="border border-black/10 p-8">
          <div className="flex items-center justify-between mb-6">
            <h2 className="font-heading font-black text-lg tracking-tight">
              {editId ? "Edit Device" : "New Device"}
            </h2>
            <button onClick={() => { setShowForm(false); setEditId(null); }}>
              <X className="w-5 h-5 opacity-30 hover:opacity-100" />
            </button>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Device Code *</label>
              <input value={form.deviceCode} onChange={(e) => setForm((f) => ({ ...f, deviceCode: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Name *</label>
              <input value={form.name} onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Location</label>
              <input value={form.location} onChange={(e) => setForm((f) => ({ ...f, location: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">IP Address *</label>
              <input value={form.ipAddress} onChange={(e) => setForm((f) => ({ ...f, ipAddress: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Port</label>
              <input type="number" value={form.port} onChange={(e) => setForm((f) => ({ ...f, port: parseInt(e.target.value) || 80 }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Serial Number</label>
              <input value={form.serialNumber} onChange={(e) => setForm((f) => ({ ...f, serialNumber: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Username</label>
              <input value={form.username} onChange={(e) => setForm((f) => ({ ...f, username: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Password</label>
              <input type="password" value={form.password} onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-[10px] font-black tracking-swiss uppercase opacity-40">Manufacturer</label>
              <input value={form.manufacturer} onChange={(e) => setForm((f) => ({ ...f, manufacturer: e.target.value }))}
                className="border border-black/10 px-3 py-2 text-sm font-mono outline-none focus:border-black/40" />
            </div>
          </div>
          <div className="flex gap-3 justify-end mt-6">
            <button onClick={() => { setShowForm(false); setEditId(null); }} className="swiss-button-outline text-xs">Cancel</button>
            <button onClick={handleSave} className="swiss-button text-xs">{editId ? "Update" : "Create"}</button>
          </div>
        </section>
      )}

      <section className="flex flex-col">
        {loading ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-20">Loading...</div>
        ) : !data || data.items.length === 0 ? (
          <div className="py-16 text-center">
            <p className="font-heading font-black text-2xl tracking-tight opacity-20">NO DEVICES</p>
            <p className="text-sm font-mono opacity-30 mt-2">Add your first attendance device to start syncing.</p>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-[auto_1fr_1fr_1fr_1fr_auto_auto] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10 items-center">
              <span className="w-4" />
              <span>Name</span>
              <span>Location</span>
              <span>IP:Port</span>
              <span>Serial</span>
              <span>Status</span>
              <span className="w-24 text-right">Actions</span>
            </div>
            {data.items.map((device) => (
              <div key={device.id}
                className="grid grid-cols-[auto_1fr_1fr_1fr_1fr_auto_auto] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors">
                <div className="w-4">
                  {device.status === "Online" ? (
                    <Wifi className="w-3 h-3 text-green-600" />
                  ) : (
                    <WifiOff className="w-3 h-3 text-red-500" />
                  )}
                </div>
                <div>
                  <div>{device.name}</div>
                  <div className="text-[10px] opacity-40">{device.deviceCode}</div>
                </div>
                <div className="opacity-60">{device.location}</div>
                <div className="opacity-60">{device.ipAddress}:{device.port}</div>
                <div className="opacity-60 text-[13px]">{device.serialNumber}</div>
                <div>
                  <span className={cn(
                    "text-[10px] font-black tracking-swiss uppercase",
                    device.status === "Online" ? "text-green-600" :
                    device.status === "Offline" ? "text-red-500" : "text-yellow-600"
                  )}>
                    {device.status}
                  </span>
                </div>
                <div className="flex gap-2 justify-end">
                  <button
                    onClick={() => handleTestConnection(device.id)}
                    disabled={testingId === device.id}
                    className="text-[10px] font-black tracking-swiss uppercase opacity-40 hover:opacity-100 disabled:opacity-20 px-2 py-1 border border-black/10"
                  >
                    {testingId === device.id ? "..." : "Ping"}
                  </button>
                  <button
                    onClick={() => handleEdit(device)}
                    className="text-[10px] font-black tracking-swiss uppercase opacity-40 hover:opacity-100 px-2 py-1 border border-black/10"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(device.id)}
                    className="text-[10px] font-black tracking-swiss uppercase opacity-40 hover:opacity-100 hover:text-red-500 px-2 py-1 border border-black/10"
                  >
                    Del
                  </button>
                </div>
              </div>
            ))}
          </>
        )}
      </section>
    </main>
  );
}
