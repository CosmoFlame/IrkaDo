// Shared i18n constants. Ukrainian is the default; English is the secondary language.
// Locale is stored in a cookie (no URL segment) so one URL serves both languages.

export const locales = ["uk", "en"] as const;
export type Locale = (typeof locales)[number];

export const defaultLocale: Locale = "uk";

export const LOCALE_COOKIE = "locale";

// One year, so a visitor's choice sticks across sessions.
export const LOCALE_COOKIE_MAX_AGE = 60 * 60 * 24 * 365;

export function isLocale(value: string | undefined | null): value is Locale {
  return value === "uk" || value === "en";
}
