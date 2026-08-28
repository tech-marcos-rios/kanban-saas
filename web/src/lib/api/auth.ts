import { apiFetch } from "../api-client";
import type { AuthResponse } from "../types";

export function register(name: string, email: string, password: string) {
  return apiFetch<AuthResponse>("/api/v1/auth/register", {
    method: "POST",
    body: { name, email, password },
    skipAuth: true,
  });
}

export function login(email: string, password: string) {
  return apiFetch<AuthResponse>("/api/v1/auth/login", {
    method: "POST",
    body: { email, password },
    skipAuth: true,
  });
}

export function logout() {
  return apiFetch<void>("/api/v1/auth/logout", { method: "POST" });
}
