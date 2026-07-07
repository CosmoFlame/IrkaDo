"use client";

import { useEffect, useState } from "react";
import { adminApi } from "@/lib/adminApi";
import { Button, EmptyState, PageHeader } from "@/components/admin/ui";
import type { AdminDownloadLog, PagedResult } from "@/types/admin";

const PAGE_SIZE = 25;

export default function DownloadLogsPage() {
  const [page, setPage] = useState(1);
  const [result, setResult] = useState<PagedResult<AdminDownloadLog> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    adminApi
      .get<PagedResult<AdminDownloadLog>>(`/admin/download-logs?page=${page}&pageSize=${PAGE_SIZE}`)
      .then(setResult)
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [page]);

  const totalPages = result ? Math.max(1, Math.ceil(result.totalCount / PAGE_SIZE)) : 1;

  return (
    <div>
      <PageHeader title="Downloads" description="Free-guide download log (read-only)." />
      {loading && <p className="text-sm text-zinc-400">Loading…</p>}
      {error && <EmptyState>{error}</EmptyState>}
      {result && result.items.length === 0 && <EmptyState>No downloads recorded yet.</EmptyState>}
      {result && result.items.length > 0 && (
        <>
          <div className="overflow-hidden rounded-2xl border border-zinc-200 bg-white">
            <table className="w-full text-left text-sm">
              <thead className="border-b border-zinc-100 bg-zinc-50 text-xs uppercase tracking-wide text-zinc-500">
                <tr>
                  <th className="px-5 py-3">Guide</th>
                  <th className="px-5 py-3">Email</th>
                  <th className="px-5 py-3">IP</th>
                  <th className="px-5 py-3">Date</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-100">
                {result.items.map((d) => (
                  <tr key={d.id}>
                    <td className="px-5 py-3 font-medium text-zinc-900">{d.guideTitle ?? "—"}</td>
                    <td className="px-5 py-3 text-zinc-500">{d.email ?? "—"}</td>
                    <td className="px-5 py-3 text-zinc-500">{d.ipAddress}</td>
                    <td className="px-5 py-3 text-zinc-500">{new Date(d.createdAt).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="mt-4 flex items-center justify-between text-sm text-zinc-500">
            <span>
              Page {page} of {totalPages} · {result.totalCount} total
            </span>
            <div className="flex gap-2">
              <Button variant="secondary" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
                Previous
              </Button>
              <Button variant="secondary" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>
                Next
              </Button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
