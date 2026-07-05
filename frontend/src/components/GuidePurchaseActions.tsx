"use client";

import { useState } from "react";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

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

  async function handleDownload() {
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
  }

  async function handleCheckout() {
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
  }

  if (isPremium) {
    return (
      <div className="mt-8 flex flex-col items-start gap-3">
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
