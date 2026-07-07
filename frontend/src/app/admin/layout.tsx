"use client";

import { useEffect, useSyncExternalStore } from "react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { ADMIN_LOGIN_PATH, clearToken, getToken } from "@/lib/adminApi";
import { cx } from "@/components/admin/ui";

// Read the stored token as an external store so the guard has no setState-in-effect and stays
// hydration-safe (server snapshot is null, then the client fills it in) — same approach as
// GuidePurchaseActions reading the query string.
function subscribeToken(callback: () => void) {
  window.addEventListener("storage", callback);
  return () => window.removeEventListener("storage", callback);
}

const NAV = [
  { href: "/admin", label: "Dashboard" },
  { href: "/admin/news", label: "News" },
  { href: "/admin/guides", label: "Travel Guides" },
  { href: "/admin/collaborations", label: "Collaborations" },
  { href: "/admin/social-links", label: "Social Links" },
  { href: "/admin/highlights", label: "Highlights" },
  { href: "/admin/home-sections", label: "Home Sections" },
  { href: "/admin/categories", label: "Categories" },
  { href: "/admin/tags", label: "Tags" },
  { href: "/admin/pages", label: "Pages" },
  { href: "/admin/media", label: "Media" },
  { href: "/admin/purchases", label: "Purchases" },
  { href: "/admin/download-logs", label: "Downloads" },
];

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const isLoginRoute = pathname === ADMIN_LOGIN_PATH;

  const token = useSyncExternalStore(subscribeToken, getToken, () => null);

  useEffect(() => {
    if (!isLoginRoute && token === null) {
      router.replace(ADMIN_LOGIN_PATH);
    }
  }, [isLoginRoute, token, router]);

  // The login page renders on its own, without the admin shell.
  if (isLoginRoute) return <>{children}</>;

  // No token: show a placeholder while the effect above redirects to login.
  if (token === null) {
    return <div className="flex min-h-screen items-center justify-center text-sm text-zinc-400">Loading…</div>;
  }

  const handleLogout = () => {
    clearToken();
    router.replace(ADMIN_LOGIN_PATH);
  };

  return (
    <div className="flex min-h-screen bg-zinc-50">
      <aside className="sticky top-0 flex h-screen w-60 shrink-0 flex-col border-r border-zinc-200 bg-white">
        <div className="border-b border-zinc-100 px-6 py-5">
          <Link href="/admin" className="text-lg font-semibold tracking-tight text-zinc-900">
            Irka_do <span className="text-zinc-400">Admin</span>
          </Link>
        </div>
        <nav className="flex-1 overflow-y-auto px-3 py-4">
          <ul className="space-y-1">
            {NAV.map((item) => {
              const active = item.href === "/admin" ? pathname === item.href : pathname.startsWith(item.href);
              return (
                <li key={item.href}>
                  <Link
                    href={item.href}
                    className={cx(
                      "block rounded-lg px-3 py-2 text-sm font-medium transition",
                      active ? "bg-zinc-900 text-white" : "text-zinc-600 hover:bg-zinc-100",
                    )}
                  >
                    {item.label}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>
        <div className="border-t border-zinc-100 p-3">
          <button
            onClick={handleLogout}
            className="w-full rounded-lg px-3 py-2 text-left text-sm font-medium text-zinc-500 transition hover:bg-zinc-100"
          >
            Sign out
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-x-hidden px-8 py-10">
        <div className="mx-auto w-full max-w-5xl">{children}</div>
      </main>
    </div>
  );
}
