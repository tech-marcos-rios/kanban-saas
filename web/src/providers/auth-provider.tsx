"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import * as authApi from "@/lib/api/auth";
import {
  clearTokens,
  getStoredUser,
  setStoredUser,
  setTokens,
  type StoredUser,
} from "@/lib/auth-storage";
import type { AuthResponse } from "@/lib/types";

interface AuthContextValue {
  user: StoredUser | null;
  isHydrated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

interface AuthState {
  user: StoredUser | null;
  isHydrated: boolean;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  // user e isHydrated viven en un solo estado: localStorage no existe en el render de
  // servidor, así que el usuario recién se puede leer una vez montado en el cliente
  // (para no desincronizar el HTML del servidor con el del cliente en la hidratación).
  const [{ user, isHydrated }, setState] = useState<AuthState>({ user: null, isHydrated: false });

  useEffect(() => {
    // localStorage no existe en el render de servidor: esta lectura solo puede pasar acá,
    // una vez montado en el cliente. No es un valor "reactivo" externo que cambie por su
    // cuenta (solo lo tocan login/register/logout, que ya actualizan el estado de React),
    // así que no aplica useSyncExternalStore — es un bootstrap de una sola vez.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setState({ user: getStoredUser(), isHydrated: true });
  }, []);

  const applyAuthResponse = (res: AuthResponse) => {
    setTokens(res.accessToken, res.refreshToken);
    const storedUser: StoredUser = { name: res.userName, email: res.email, role: res.role };
    setStoredUser(storedUser);
    setState({ user: storedUser, isHydrated: true });
  };

  const login = async (email: string, password: string) => {
    applyAuthResponse(await authApi.login(email, password));
  };

  const register = async (name: string, email: string, password: string) => {
    applyAuthResponse(await authApi.register(name, email, password));
  };

  const logout = async () => {
    try {
      await authApi.logout();
    } finally {
      clearTokens();
      setState({ user: null, isHydrated: true });
    }
  };

  return (
    <AuthContext.Provider value={{ user, isHydrated, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return ctx;
}
