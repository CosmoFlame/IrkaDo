"use client";

import { useRouter } from "next/navigation";

/**
 * Small icon-only "go back" control (left arrow) for detail pages.
 * Uses the browser history when possible and falls back to a sensible route.
 */
export function BackButton({
  label,
  fallbackHref = "/guides",
  className = "",
}: {
  label: string;
  fallbackHref?: string;
  className?: string;
}) {
  const router = useRouter();

  const handleClick = () => {
    // history.length > 1 means there is somewhere to go back to within the app.
    if (typeof window !== "undefined" && window.history.length > 1) {
      router.back();
    } else {
      router.push(fallbackHref);
    }
  };

  return (
    <button
      type="button"
      onClick={handleClick}
      aria-label={label}
      title={label}
      className={`inline-flex h-10 w-10 items-center justify-center rounded-full bg-white/90 text-zinc-700 shadow-sm ring-1 ring-zinc-900/10 backdrop-blur transition hover:bg-white hover:text-zinc-900 ${className}`}
    >
      <svg
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth={2}
        strokeLinecap="round"
        strokeLinejoin="round"
        className="h-5 w-5"
        aria-hidden
      >
        <path d="M15 18l-6-6 6-6" />
      </svg>
    </button>
  );
}
