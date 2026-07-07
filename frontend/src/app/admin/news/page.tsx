"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { useAdminData } from "@/components/admin/useAdminData";
import { EmptyState, LinkButton, PageHeader, StatusPill } from "@/components/admin/ui";
import type { AdminNewsListItem } from "@/types/admin";

export default function NewsListPage() {
  const { data, error, loading, reload } = useAdminData<AdminNewsListItem[]>("/admin/news");
  const router = useRouter();
  const [busy, setBusy] = useState<string | null>(null);

  const handleDelete = async (item: AdminNewsListItem) => {
    if (!confirm(`Delete "${item.title}"? This cannot be undone.`)) return;
    setBusy(item.id);
    try {
      await adminApi.del(`/admin/news/${item.id}`);
      reload();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Delete failed.");
    } finally {
      setBusy(null);
    }
  };

  return (
    <div>
      <PageHeader
        title="News"
        description="Articles shown on the News page and home preview."
        actions={<LinkButton href="/admin/news/new">New article</LinkButton>}
      />
      {loading && <p className="text-sm text-zinc-400">Loading…</p>}
      {error && <EmptyState>{error}</EmptyState>}
      {data && data.length === 0 && <EmptyState>No articles yet.</EmptyState>}
      {data && data.length > 0 && (
        <div className="overflow-hidden rounded-2xl border border-zinc-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-zinc-100 bg-zinc-50 text-xs uppercase tracking-wide text-zinc-500">
              <tr>
                <th className="px-5 py-3">Title</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3">Category</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {data.map((item) => (
                <tr key={item.id} className="hover:bg-zinc-50/60">
                  <td className="px-5 py-3 font-medium text-zinc-900">{item.title}</td>
                  <td className="px-5 py-3">
                    <StatusPill published={item.isPublished} />
                  </td>
                  <td className="px-5 py-3 text-zinc-500">{item.category ?? "—"}</td>
                  <td className="px-5 py-3 text-right">
                    <button
                      onClick={() => router.push(`/admin/news/${item.id}`)}
                      className="mr-3 text-sm font-medium text-zinc-700 hover:text-zinc-900"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(item)}
                      disabled={busy === item.id}
                      className="text-sm font-medium text-red-600 hover:text-red-700 disabled:opacity-50"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
