"use client";

import { useCallback, useEffect, useState, useSyncExternalStore } from "react";
import type { Locale } from "@/i18n/config";
import { getDictionary } from "@/i18n/dictionaries";
import type { PurchaseStatusResponse } from "@/types/api";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

// How long the confirmation screen keeps polling for the webhook to mark the purchase delivered
// before falling back to "check your email" (webhooks are usually near-instant but can lag).
const POLL_INTERVAL_MS = 2500;
const POLL_TIMEOUT_MS = 45_000;

type PollResult =
  | { phase: "polling" }
  | { phase: "ready"; downloadUrl: string; guideTitle: string }
  | { phase: "pending" } // still processing after the poll timeout
  | { phase: "failed" };

const noopSubscribe = () => () => {};

// Reads the current query string without an effect so the page stays statically generated
// (no useSearchParams/Suspense bailout) and hydration stays clean (server snapshot is empty).
function useSearchString(): string {
  return useSyncExternalStore(
    noopSubscribe,
    () => window.location.search,
    () => "",
  );
}

export function GuidePurchaseActions({
  slug,
  isPremium,
  priceAmount,
  priceCurrency,
  hasFile,
  locale,
}: {
  slug: string;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
  hasFile: boolean;
  locale: Locale;
}) {
  const t = getDictionary(locale).purchase;
  const [status, setStatus] = useState<"idle" | "loading" | "error" | "rate-limited">("idle");
  const [result, setResult] = useState<PollResult>({ phase: "polling" });

  // Stripe appends ?purchase=success&session_id=… (or ?purchase=cancelled) on redirect.
  const params = new URLSearchParams(useSearchString());
  const outcome = params.get("purchase");
  const sessionId = params.get("session_id");
  const isSuccess = outcome === "success" && !!sessionId;
  const isCancelled = outcome === "cancelled";

  // Poll the backend for delivery status after a successful checkout.
  useEffect(() => {
    if (!isSuccess || !sessionId) return;

    let active = true;
    let timer = 0;
    const startedAt = Date.now();

    async function poll() {
      try {
        const res = await fetch(
          `${API_BASE_URL}/api/v1/purchases/${encodeURIComponent(sessionId!)}`,
        );
        if (active && res.ok) {
          const data = (await res.json()) as PurchaseStatusResponse;
          if (data.status === "completed" && data.downloadUrl) {
            setResult({
              phase: "ready",
              downloadUrl: data.downloadUrl,
              guideTitle: data.guideTitle,
            });
            return;
          }
          if (data.status === "failed") {
            setResult({ phase: "failed" });
            return;
          }
        }
      } catch {
        // transient — keep polling until the timeout
      }

      if (!active) return;
      if (Date.now() - startedAt >= POLL_TIMEOUT_MS) {
        setResult({ phase: "pending" });
        return;
      }
      timer = window.setTimeout(poll, POLL_INTERVAL_MS);
    }

    timer = window.setTimeout(poll, 0);
    return () => {
      active = false;
      window.clearTimeout(timer);
    };
  }, [isSuccess, sessionId]);

  const handleDownload = useCallback(async () => {
    setStatus("loading");
    try {
      const res = await fetch(`${API_BASE_URL}/api/v1/guides/${slug}/download`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({}),
      });
      if (res.status === 429) {
        setStatus("rate-limited");
        return;
      }
      if (!res.ok) throw new Error("download failed");
      const data = await res.json();
      window.location.href = data.downloadUrl;
      setStatus("idle");
    } catch {
      setStatus("error");
    }
  }, [slug]);

  const handleCheckout = useCallback(async () => {
    setStatus("loading");
    try {
      const res = await fetch(`${API_BASE_URL}/api/v1/guides/${slug}/checkout`, {
        method: "POST",
      });
      if (!res.ok) throw new Error("checkout failed");
      const data = await res.json();
      window.location.href = data.checkoutUrl;
    } catch {
      setStatus("error");
    }
  }, [slug]);

  // Post-checkout confirmation takes over the action area after a successful redirect.
  if (isSuccess) {
    if (result.phase === "ready") {
      return (
        <div className="mt-8 rounded-2xl border border-emerald-200 bg-emerald-50 p-6">
          <h2 className="text-lg font-semibold text-emerald-900">{t.readyTitle}</h2>
          <p className="mt-1 text-sm text-emerald-800">{t.readyBody}</p>
          <a
            href={result.downloadUrl}
            className="mt-4 inline-block rounded-full bg-emerald-600 px-6 py-3 text-sm font-semibold text-white transition hover:bg-emerald-500"
          >
            {t.download}
          </a>
        </div>
      );
    }

    if (result.phase === "failed") {
      return (
        <div className="mt-8 rounded-2xl border border-red-200 bg-red-50 p-6">
          <h2 className="text-lg font-semibold text-red-900">{t.failedTitle}</h2>
          <p className="mt-1 text-sm text-red-800">{t.failedBody}</p>
          <button
            onClick={handleCheckout}
            disabled={status === "loading"}
            className="mt-4 rounded-full bg-amber-500 px-6 py-3 text-sm font-semibold text-zinc-900 transition hover:bg-amber-400 disabled:opacity-60"
          >
            {status === "loading" ? t.redirecting : t.tryAgain}
          </button>
        </div>
      );
    }

    if (result.phase === "pending") {
      return (
        <div className="mt-8 rounded-2xl border border-amber-200 bg-amber-50 p-6">
          <h2 className="text-lg font-semibold text-amber-900">{t.pendingTitle}</h2>
          <p className="mt-1 text-sm text-amber-800">{t.pendingBody}</p>
        </div>
      );
    }

    return (
      <div className="mt-8 rounded-2xl border border-zinc-200 bg-zinc-50 p-6">
        <h2 className="text-lg font-semibold text-zinc-900">{t.confirmingTitle}</h2>
        <p className="mt-1 text-sm text-zinc-600">{t.confirmingBody}</p>
      </div>
    );
  }

  // No downloadable file attached yet — there is nothing to buy or download.
  if (!hasFile) return null;

  if (isPremium) {
    return (
      <div className="mt-8 flex flex-col items-start gap-3">
        {isCancelled && (
          <p className="text-sm text-zinc-600">{t.cancelled}</p>
        )}
        <p className="text-2xl font-semibold text-zinc-900">
          {priceCurrency} {priceAmount}
        </p>
        <button
          onClick={handleCheckout}
          disabled={status === "loading"}
          className="rounded-full bg-amber-500 px-6 py-3 text-sm font-semibold text-zinc-900 transition hover:bg-amber-400 disabled:opacity-60"
        >
          {status === "loading" ? t.redirecting : t.buy}
        </button>
        {status === "error" && (
          <p className="text-sm text-red-600">{t.somethingWrong}</p>
        )}
      </div>
    );
  }

  return (
    <div className="mt-8">
      <button
        onClick={handleDownload}
        disabled={status === "loading"}
        className="rounded-full bg-emerald-600 px-6 py-3 text-sm font-semibold text-white transition hover:bg-emerald-500 disabled:opacity-60"
      >
        {status === "loading" ? t.preparing : t.downloadFree}
      </button>
      {status === "error" && (
        <p className="mt-2 text-sm text-red-600">{t.somethingWrong}</p>
      )}
      {status === "rate-limited" && (
        <p className="mt-2 text-sm text-red-600">{t.rateLimited}</p>
      )}
    </div>
  );
}
