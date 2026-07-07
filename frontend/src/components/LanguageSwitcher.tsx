"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";
import {
  LOCALE_COOKIE,
  LOCALE_COOKIE_MAX_AGE,
  locales,
  type Locale,
} from "@/i18n/config";

const labels: Record<Locale, string> = { uk: "UA", en: "EN" };

/**
 * Toggles the content language. Persists the choice in the `locale` cookie and refreshes the
 * route so the server components re-render with the new language (content + static copy).
 */
export function LanguageSwitcher({ locale, label }: { locale: Locale; label: string }) {
  const router = useRouter();
  const [pending, startTransition] = useTransition();

  function select(next: Locale) {
    if (next === locale) return;
    // Persisting the choice is a legitimate event-handler side effect; the compiler's
    // immutability rule mis-reads writing document.cookie as mutating external state.
    // eslint-disable-next-line react-hooks/immutability
    document.cookie = `${LOCALE_COOKIE}=${next}; path=/; max-age=${LOCALE_COOKIE_MAX_AGE}; samesite=lax`;
    startTransition(() => router.refresh());
  }

  return (
    <div
      role="group"
      aria-label={label}
      className="inline-flex items-center rounded-full border border-zinc-200 p-0.5 text-xs font-semibold"
    >
      {locales.map((code) => {
        const active = code === locale;
        return (
          <button
            key={code}
            type="button"
            onClick={() => select(code)}
            disabled={pending}
            aria-pressed={active}
            className={`rounded-full px-2.5 py-1 transition ${
              active ? "bg-zinc-900 text-white" : "text-zinc-500 hover:text-zinc-900"
            } disabled:opacity-60`}
          >
            {labels[code]}
          </button>
        );
      })}
    </div>
  );
}
