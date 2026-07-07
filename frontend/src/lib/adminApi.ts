// Client-side admin API wrapper. Attaches the bearer token stored after login and
// clears it + bounces to the login page on 401. Import only from Client Components.

import type { AdminLoginResponse } from "@/types/admin";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";
const TOKEN_KEY = "irkado_admin_token";
export const ADMIN_LOGIN_PATH = "/admin/login";

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return window.localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
  window.localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(): void {
  window.localStorage.removeItem(TOKEN_KEY);
}

export class AdminApiError extends Error {
  constructor(
    public status: number,
    message: string,
  ) {
    super(message);
  }
}

async function readError(res: Response): Promise<string> {
  try {
    const text = await res.text();
    if (!text) return `Request failed (${res.status})`;
    try {
      const parsed = JSON.parse(text);
      // ASP.NET ProblemDetails / ModelState, or a plain string body.
      if (typeof parsed === "string") return parsed;
      if (parsed.title) return parsed.title as string;
      if (parsed.errors) return Object.values(parsed.errors).flat().join(" ");
      return text;
    } catch {
      return text;
    }
  } catch {
    return `Request failed (${res.status})`;
  }
}

function handleUnauthorized(): never {
  clearToken();
  if (typeof window !== "undefined") {
    window.location.href = ADMIN_LOGIN_PATH;
  }
  throw new AdminApiError(401, "Session expired. Please sign in again.");
}

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
  const token = getToken();
  const res = await fetch(`${API_BASE_URL}/api/v1${path}`, {
    method,
    headers: {
      ...(body !== undefined ? { "Content-Type": "application/json" } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body !== undefined ? JSON.stringify(body) : undefined,
    cache: "no-store",
  });

  if (res.status === 401) handleUnauthorized();
  if (!res.ok) throw new AdminApiError(res.status, await readError(res));
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

export const adminApi = {
  get: <T>(path: string) => request<T>("GET", path),
  post: <T>(path: string, body: unknown) => request<T>("POST", path, body),
  put: <T>(path: string, body: unknown) => request<T>("PUT", path, body),
  del: (path: string) => request<void>("DELETE", path),

  async login(password: string): Promise<AdminLoginResponse> {
    const res = await fetch(`${API_BASE_URL}/api/v1/admin/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ password }),
    });
    if (res.status === 401) throw new AdminApiError(401, "Incorrect password.");
    if (res.status === 429) throw new AdminApiError(429, "Too many attempts. Try again shortly.");
    if (!res.ok) throw new AdminApiError(res.status, await readError(res));
    return (await res.json()) as AdminLoginResponse;
  },

  async upload<T>(path: string, file: File, fields?: Record<string, string>): Promise<T> {
    const token = getToken();
    const form = new FormData();
    form.append("file", file);
    for (const [k, v] of Object.entries(fields ?? {})) form.append(k, v);

    const res = await fetch(`${API_BASE_URL}/api/v1${path}`, {
      method: "POST",
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      body: form,
    });
    if (res.status === 401) handleUnauthorized();
    if (!res.ok) throw new AdminApiError(res.status, await readError(res));
    return (await res.json()) as T;
  },
};
