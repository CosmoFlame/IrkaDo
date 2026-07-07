"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import type { AdminCategory } from "@/types/admin";

export default function CategoriesListPage() {
  return (
    <ResourceList<AdminCategory>
      title="Categories"
      description="Used to group news articles."
      apiPath="/admin/categories"
      uiBasePath="/admin/categories"
      newLabel="New category"
      rowTitle={(c) => c.name}
      emptyText="No categories yet."
      columns={[
        { header: "Name", render: (c) => <span className="font-medium text-zinc-900">{c.name}</span> },
        { header: "Slug", render: (c) => <span className="text-zinc-500">{c.slug}</span> },
      ]}
    />
  );
}
