"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import type { AdminPage } from "@/types/admin";

export default function PagesListPage() {
  return (
    <ResourceList<AdminPage>
      title="Pages"
      description="Per-page titles and SEO metadata."
      apiPath="/admin/pages"
      uiBasePath="/admin/pages"
      newLabel="New page"
      rowTitle={(p) => p.title}
      emptyText="No pages yet."
      columns={[
        { header: "Title", render: (p) => <span className="font-medium text-zinc-900">{p.title}</span> },
        { header: "Slug", render: (p) => <span className="text-zinc-500">/{p.slug}</span> },
        { header: "Meta title", render: (p) => <span className="text-zinc-500">{p.metaTitle ?? "—"}</span> },
      ]}
    />
  );
}
