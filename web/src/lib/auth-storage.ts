// Los tokens y el usuario viven en localStorage: simple y alcanza para un portfolio piece.
// No es el approach más seguro (httpOnly cookie sería mejor contra XSS), pero mantiene
// el cliente stateless y calza con cómo el hub de SignalR ya espera el JWT en el browser.
const ACCESS_TOKEN_KEY = "kanban_access_token";
const REFRESH_TOKEN_KEY = "kanban_refresh_token";
const USER_KEY = "kanban_user";

export interface StoredUser {
  name: string;
  email: string;
  role: string;
}

export function getAccessToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function getStoredUser(): StoredUser | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(USER_KEY);
  return raw ? (JSON.parse(raw) as StoredUser) : null;
}

export function setTokens(accessToken: string, refreshToken: string) {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
}

export function setStoredUser(user: StoredUser) {
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function clearTokens() {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}
