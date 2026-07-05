"use client";

import { useState, useEffect, useCallback } from "react";
import { Search } from "lucide-react";
import { api } from "@/lib/api-client";
import { cn } from "@/lib/utils";

type DeptEmployee = {
  id: string;
  employeeCode: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  departmentName: string;
  positionName: string;
};

export default function ManagerEmployeesPage() {
  const [employees, setEmployees] = useState<DeptEmployee[]>([]);
  const [loading, setLoading] = useState(true);
  const [keyword, setKeyword] = useState("");

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const list = await api.get<DeptEmployee[]>("/manager/employees");
      setEmployees(list);
    } catch { /* ignore */ }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const filtered = employees.filter((e) =>
    !keyword || e.fullName.toLowerCase().includes(keyword.toLowerCase()) ||
    e.employeeCode.toLowerCase().includes(keyword.toLowerCase())
  );

  return (
    <main className="flex-1 flex flex-col overflow-auto bg-white p-8 lg:p-20 gap-12">
      <section className="flex flex-col gap-4 mt-auto">
        <p className="font-heading font-black tracking-swiss text-[10px] opacity-40 uppercase">
          MANAGER — DEPARTMENT
        </p>
        <div className="flex items-center justify-between gap-4">
          <h1 className="font-heading font-black text-[clamp(2rem,5vw,60px)] leading-[0.85] tracking-tighter">
            Department Employees
          </h1>
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 opacity-30" />
            <input
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              placeholder="Search..."
              className="border border-black/10 pl-9 pr-3 py-2 text-sm font-mono outline-none focus:border-black/40 transition-colors w-48"
            />
          </div>
        </div>
      </section>

      <section className="flex flex-col">
        <div className="grid grid-cols-[1fr_1fr_1fr_1fr_1fr] gap-4 px-4 py-3 text-[10px] font-heading font-black tracking-swiss uppercase opacity-40 border-b border-black/10">
          <span>Code</span>
          <span>Name</span>
          <span>Email</span>
          <span>Position</span>
          <span className="text-right">Status</span>
        </div>
        {loading ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-20">Loading...</div>
        ) : filtered.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm font-mono opacity-30">No employees found</div>
        ) : filtered.map((e) => (
          <div key={e.id} className="grid grid-cols-[1fr_1fr_1fr_1fr_1fr] gap-4 px-4 py-4 items-center border-b border-black/5 text-sm font-mono hover:bg-black/[0.02] transition-colors">
            <div className="opacity-60">{e.employeeCode}</div>
            <div>{e.fullName}</div>
            <div className="opacity-60 text-[13px]">{e.email}</div>
            <div>{e.positionName}</div>
            <div className="flex justify-end">
              <span className={cn("inline-block w-2 h-2 rounded-full", e.isActive ? "bg-black" : "bg-black/20")} />
            </div>
          </div>
        ))}
      </section>
    </main>
  );
}
