"use client";

import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from "react";
import { useRouter } from "next/navigation";
import { api, setTokens, clearTokens, getTokens } from "./api-client";

type User = {
  id: string;
  email: string;
  role: string;
};

type AuthContextType = {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (data: RegisterData) => Promise<void>;
  logout: () => void;
};

type RegisterData = {
  email: string;
  password: string;
  fullName: string;
  employeeCode: string;
  departmentId?: string;
  positionId?: string;
};

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const { accessToken } = getTokens();
    if (accessToken) {
      setUser({
        id: localStorage.getItem("userId") ?? "",
        email: localStorage.getItem("userEmail") ?? "",
        role: localStorage.getItem("userRole") ?? "",
      });
    }
    setLoading(false);
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const data = await api.post<{
        accessToken: string;
        refreshToken: string;
        userId?: string;
        role?: string;
        email?: string;
      }>("/auth/login", { email, password });

      setTokens(data.accessToken, data.refreshToken);

      const userId = data.userId ?? "";
      const userRole = data.role ?? "";
      const userEmail = data.email ?? email;

      localStorage.setItem("userId", userId);
      localStorage.setItem("userRole", userRole);
      localStorage.setItem("userEmail", userEmail);

      setUser({ id: userId, email: userEmail, role: userRole });
      router.push("/");
    },
    [router]
  );

  const register = useCallback(
    async (registerData: RegisterData) => {
      await api.post("/auth/register", registerData);
      router.push("/auth/login");
    },
    [router]
  );

  const logout = useCallback(() => {
    const { refreshToken } = getTokens();
    if (refreshToken) {
      api.post("/auth/logout", { refreshToken }).catch(() => {});
    }
    clearTokens();
    setUser(null);
    router.push("/auth/login");
  }, [router]);

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
