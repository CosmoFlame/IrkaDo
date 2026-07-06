"use client";

import { useCallback, useEffect, useState, useSyncExternalStore } from "react";
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
}: {
  slug: string;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
}) {
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
          <h2 className="text-lg font-semibold text-emerald-900">Thank you for your purchase!</h2>
          <p className="mt-1 text-sm text-emerald-800">
            Your guide is ready. We&apos;ve also emailed you a copy of this link.
          </p>
          <a
            href={result.downloadUrl}
            className="mt-4 inline-block rounded-full bg-emerald-600 px-6 py-3 text-sm font-semibold text-white transition hover:bg-emerald-500"
          >
            Download your guide
          </a>
        </div>
      );
    }

    if (result.phase === "failed") {
      return (
        <div className="mt-8 rounded-2xl border border-red-200 bg-red-50 p-6">
          <h2 className="text-lg font-semibold text-red-900">Payment didn&apos;t go through</h2>
          <p className="mt-1 text-sm text-red-800">
            Your card was not charged. You can try again below.
          </p>
          <button
            onClick={handleCheckout}
            disabled={status === "loading"}
            className="mt-4 rounded-full bg-amber-500 px-6 py-3 text-sm font-semibold text-zinc-900 transition hover:bg-amber-400 disabled:opacity-60"
          >
            {status === "loading" ? "Redirecting…" : "Try Again"}
          </button>
        </div>
      );
    }

    if (result.phase === "pending") {
      return (
        <div className="mt-8 rounded-2xl border border-amber-200 bg-amber-50 p-6">
          <h2 className="text-lg font-semibold text-amber-900">Your payment is being processed</h2>
          <p className="mt-1 text-sm text-amber-800">
            Thanks for your purchase! Your download link is on its way to your email — it should
            arrive within a few minutes.
          </p>
        </div>
      );
    }

    return (
      <div className="mt-8 rounded-2xl border border-zinc-200 bg-zinc-50 p-6">
        <h2 className="text-lg font-semibold text-zinc-900">Confirming your payment…</h2>
        <p className="mt-1 text-sm text-zinc-600">
          This only takes a moment. Please don&apos;t close this page.
        </p>
      </div>
    );
  }

  if (isPremium) {
    return (
      <div className="mt-8 flex flex-col items-start gap-3">
        {isCancelled && (
          <p className="text-sm text-zinc-600">
            Checkout cancelled — no charge was made. You can pick up where you left off below.
          </p>
        )}
        <p className="text-2xl font-semibold text-zinc-900">
          {priceCurrency} {priceAmount}
        </p>
        <button
          onClick={handleCheckout}
          disabled={status === "loading"}
          className="rounded-full bg-amber-500 px-6 py-3 text-sm font-semibold text-zinc-900 transition hover:bg-amber-400 disabled:opacity-60"
        >
          {status === "loading" ? "Redirecting…" : "Buy This Guide"}
        </button>
        {status === "error" && (
          <p className="text-sm text-red-600">Something went wrong. Please try again.</p>
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
        {status === "loading" ? "Preparing download…" : "Download Free Guide"}
      </button>
      {status === "error" && (
        <p className="mt-2 text-sm text-red-600">Something went wrong. Please try again.</p>
      )}
      {status === "rate-limited" && (
        <p className="mt-2 text-sm text-red-600">
          Too many attempts — please try again in a minute.
        </p>
      )}
    </div>
  );
}
