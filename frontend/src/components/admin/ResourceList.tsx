"use client";

import { type ReactNode, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { useAdminData } from "@/components/admin/useAdminData";
import { EmptyState, LinkButton, PageHeader } from "@/components/admin/ui";

export interface Column<T> {
  header: string;
  render: (item: T) => ReactNode;
  align?: "left" | "right";
}

interface Props<T extends { id: string }> {
  title: string;
  description?: string;
  apiPath: string; // e.g. /admin/categories
  uiBasePath: string; // e.g. /admin/categories
  newLabel?: string;
  columns: Column<T>[];
  rowTitle: (item: T) => string;
  canEdit?: boolean;
  canCreate?: boolean;
  canDelete?: boolean;
  emptyText?: string;
}

export function ResourceList<T extends { id: string }>({
  title,
  description,
  apiPath,
  uiBasePath,
  newLabel = "New",
  columns,
  rowTitle,
  canEdit = true,
  canCreate = true,
  canDelete = true,
  emptyText = "Nothing here yet.",
}: Props<T>) {
  const { data, error, loading, reload } = useAdminData<T[]>(apiPath);
  const router = useRouter();
  const [busy, setBusy] = useState<string | null>(null);

  const handleDelete = async (item: T) => {
    if (!confirm(`Delete "${rowTitle(item)}"? This cannot be undone.`)) return;
    setBusy(item.id);
    try {
      await adminApi.del(`${apiPath}/${item.id}`);
      reload();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Delete failed.");
    } finally {
      setBusy(null);
    }
  };

  const showActions = canEdit || canDelete;

  return (
    <div>
      <PageHeader
        title={title}
        description={description}
        actions={canCreate ? <LinkButton href={`${uiBasePath}/new`}>{newLabel}</LinkButton> : undefined}
      />
      {loading && <p className="text-sm text-zinc-400">Loading…</p>}
      {error && <EmptyState>{error}</EmptyState>}
      {data && data.length === 0 && <EmptyState>{emptyText}</EmptyState>}
      {data && data.length > 0 && (
        <div className="overflow-hidden rounded-2xl border border-zinc-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-zinc-100 bg-zinc-50 text-xs uppercase tracking-wide text-zinc-500">
              <tr>
                {columns.map((c) => (
                  <th key={c.header} className={c.align === "right" ? "px-5 py-3 text-right" : "px-5 py-3"}>
                    {c.header}
                  </th>
                ))}
                {showActions && <th className="px-5 py-3 text-right">Actions</th>}
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-100">
              {data.map((item) => (
                <tr key={item.id} className="hover:bg-zinc-50/60">
                  {columns.map((c) => (
                    <td key={c.header} className={c.align === "right" ? "px-5 py-3 text-right" : "px-5 py-3"}>
                      {c.render(item)}
                    </td>
                  ))}
                  {showActions && (
                    <td className="px-5 py-3 text-right">
                      {canEdit && (
                        <button
                          onClick={() => router.push(`${uiBasePath}/${item.id}`)}
                          className="mr-3 text-sm font-medium text-zinc-700 hover:text-zinc-900"
                        >
                          Edit
                        </button>
                      )}
                      {canDelete && (
                        <button
                          onClick={() => handleDelete(item)}
                          disabled={busy === item.id}
                          className="text-sm font-medium text-red-600 hover:text-red-700 disabled:opacity-50"
                        >
                          Delete
                        </button>
                      )}
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
