"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const links = [
  { href: "/", label: "Home" },
  { href: "/guides", label: "Travel Guides" },
  { href: "/news", label: "News" },
  { href: "/#contact", label: "Contact" },
];

export function SiteNav() {
  const pathname = usePathname();
  // The admin area has its own chrome; keep the public nav off those routes.
  if (pathname?.startsWith("/admin")) return null;

  return (
    <header className="sticky top-0 z-50 border-b border-zinc-900/5 bg-white/80 backdrop-blur">
      <nav className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
        <Link href="/" className="text-lg font-semibold tracking-tight text-zinc-900">
          Irka_do
        </Link>
        <ul className="flex items-center gap-6 text-sm font-medium text-zinc-600">
          {links.map((link) => (
            <li key={link.href}>
              <Link href={link.href} className="transition hover:text-zinc-900">
                {link.label}
              </Link>
            </li>
          ))}
        </ul>
      </nav>
    </header>
  );
}
