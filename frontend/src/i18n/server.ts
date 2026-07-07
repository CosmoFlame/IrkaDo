import { cookies } from "next/headers";
import { defaultLocale, isLocale, LOCALE_COOKIE, type Locale } from "./config";

/**
 * Resolves the active locale from the `locale` cookie, defaulting to Ukrainian.
 *
 * Request-scoped only: call from pages, layouts, and `generateMetadata`. Do NOT call from
 * `generateStaticParams` or `sitemap` (build-time contexts where `cookies()` is unavailable) —
 * slug enumeration is language-neutral anyway.
 */
export async function getLocale(): Promise<Locale> {
  const store = await cookies();
  const value = store.get(LOCALE_COOKIE)?.value;
  return isLocale(value) ? value : defaultLocale;
}
