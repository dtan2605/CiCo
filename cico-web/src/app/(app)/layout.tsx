"use client";

import { useEffect } from "react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { Clock, Calendar, Bell, User, History, Shield, Send, Megaphone, Users, UserCheck, Monitor, LogOut } from "lucide-react";
import { AuthProvider, useAuth } from "@/lib/auth-context";
import { cn } from "@/lib/utils";

function AppShell({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const { user, loading, logout } = useAuth();

  useEffect(() => {
    if (!loading && !user) router.push("/auth/login");
  }, [loading, user, router]);

  if (loading || !user) return null;

  const role = user?.role ?? "";
  const isAdmin = role === "Admin";
  const isManager = role === "Manager";

  const navItems = [
    { icon: Clock, label: "Dashboard", href: "/" },
    { icon: History, label: "History", href: "/history" },
    ...(isAdmin
      ? [{ icon: Calendar, label: "Schedules", href: "/admin/schedules" }]
      : [{ icon: Calendar, label: "Schedule", href: "/schedule" }]
    ),
    ...(!isAdmin ? [
      { icon: Send, label: "Requests", href: "/requests" },
      { icon: Bell, label: "Notifications", href: "/notifications" },
    ] : []),
    { icon: User, label: "Profile", href: "/profile" },
    ...(isAdmin ? [
      { icon: Monitor, label: "Devices", href: "/admin/devices" },
      { icon: Shield, label: "Users", href: "/admin/users" },
      { icon: Send, label: "Req Admin", href: "/admin/requests" },
      { icon: UserCheck, label: "Profile Req", href: "/admin/profile-requests" },
      { icon: Megaphone, label: "Notify", href: "/admin/notifications" },
    ] : []),
    ...(isManager ? [
      { icon: Users, label: "Dept Employees", href: "/manager/employees" },
    ] : []),
  ];

  return (
    <div className="flex h-screen overflow-hidden">
      <aside className="w-16 bg-black flex flex-col items-center py-8 gap-8 shrink-0 z-50">
        <Link
          href="/"
          className="w-8 h-8 bg-white hover:scale-110 transition-transform flex items-center justify-center"
          aria-label="CiCo Dashboard Home"
        >
          <span className="text-[8px] font-black text-black tracking-tighter leading-none">IO</span>
        </Link>
        <nav className="flex flex-col gap-2">
          {navItems.map((item) => {
            const isActive = pathname === item.href;
            return (
              <Link
                key={item.href}
                href={item.href}
                aria-label={item.label}
                className={cn(
                  "icon-rail-item text-white rounded-none",
                  isActive
                    ? "icon-rail-item-active"
                    : "opacity-40 hover:opacity-100"
                )}
              >
                <item.icon className="w-6 h-6" />
              </Link>
            );
          })}
        </nav>
        <div className="mt-auto">
          <button
            onClick={logout}
            aria-label="Logout"
            className="icon-rail-item text-white rounded-none opacity-20 hover:opacity-100 cursor-pointer"
          >
            <LogOut className="w-5 h-5" />
          </button>
        </div>
      </aside>
      <div className="flex-1 flex flex-col overflow-hidden relative">
        {children}
      </div>
    </div>
  );
}

export default function AppLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthProvider>
      <AppShell>{children}</AppShell>
    </AuthProvider>
  );
}
