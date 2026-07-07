"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import type { AdminTag } from "@/types/admin";

export default function TagsListPage() {
  return (
    <ResourceList<AdminTag>
      title="Tags"
      description="Optional labels attached to news articles."
      apiPath="/admin/tags"
      uiBasePath="/admin/tags"
      newLabel="New tag"
      rowTitle={(t) => t.name}
      emptyText="No tags yet."
      columns={[
        { header: "Name", render: (t) => <span className="font-medium text-zinc-900">{t.name}</span> },
        { header: "Slug", render: (t) => <span className="text-zinc-500">{t.slug}</span> },
      ]}
    />
  );
}
