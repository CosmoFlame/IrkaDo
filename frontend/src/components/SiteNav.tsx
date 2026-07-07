"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";

const links = [
  { href: "/", label: "Home" },
  { href: "/guides", label: "Travel Guides" },
  { href: "/news", label: "News" },
  { href: "/#contact", label: "Contact" },
];

function isActive(pathname: string, href: string) {
  if (href === "/") return pathname === "/";
  // Match section links (e.g. /guides and /guides/[slug]) but ignore hash-only targets.
  const base = href.split("#")[0];
  if (!base || base === "/") return false;
  return pathname === base || pathname.startsWith(`${base}/`);
}

export function SiteNav() {
  const pathname = usePathname();
  const [open, setOpen] = useState(false);

  // The admin area has its own chrome; keep the public nav off those routes.
  // (Placed after all hooks so the early return doesn't violate the rules of hooks.)
  if (pathname?.startsWith("/admin")) return null;

  return (
    <header className="sticky top-0 z-50 border-b border-zinc-900/5 bg-white/80 backdrop-blur">
      <nav
        aria-label="Main"
        className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4"
      >
        <Link
          href="/"
          className="rounded-sm text-lg font-semibold tracking-tight text-zinc-900"
        >
          Irka_do
        </Link>

        {/* Desktop links */}
        <ul className="hidden items-center gap-6 text-sm font-medium text-zinc-600 sm:flex">
          {links.map((link) => {
            const active = isActive(pathname, link.href);
            return (
              <li key={link.href}>
                <Link
                  href={link.href}
                  aria-current={active ? "page" : undefined}
                  className={`rounded-sm transition hover:text-zinc-900 ${
                    active ? "text-zinc-900" : ""
                  }`}
                >
                  {link.label}
                </Link>
              </li>
            );
          })}
        </ul>

        {/* Mobile menu toggle */}
        <button
          type="button"
          onClick={() => setOpen((v) => !v)}
          aria-expanded={open}
          aria-controls="mobile-menu"
          aria-label={open ? "Close menu" : "Open menu"}
          className="inline-flex h-10 w-10 items-center justify-center rounded-full text-zinc-700 transition hover:bg-zinc-100 sm:hidden"
        >
          <svg
            width="22"
            height="22"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            aria-hidden="true"
          >
            {open ? (
              <path d="M6 6l12 12M18 6L6 18" />
            ) : (
              <path d="M4 7h16M4 12h16M4 17h16" />
            )}
          </svg>
        </button>
      </nav>

      {/* Mobile menu panel */}
      {open && (
        <ul
          id="mobile-menu"
          className="flex flex-col gap-1 border-t border-zinc-900/5 px-4 py-3 text-sm font-medium text-zinc-700 sm:hidden"
        >
          {links.map((link) => {
            const active = isActive(pathname, link.href);
            return (
              <li key={link.href}>
                <Link
                  href={link.href}
                  onClick={() => setOpen(false)}
                  aria-current={active ? "page" : undefined}
                  className={`block rounded-lg px-3 py-2 transition hover:bg-zinc-100 ${
                    active ? "bg-zinc-100 text-zinc-900" : ""
                  }`}
                >
                  {link.label}
                </Link>
              </li>
            );
          })}
        </ul>
      )}
    </header>
  );
}
