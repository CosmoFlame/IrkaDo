"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import { StatusPill } from "@/components/admin/ui";
import type { AdminGuideListItem } from "@/types/admin";

export default function GuidesListPage() {
  return (
    <ResourceList<AdminGuideListItem>
      title="Travel Guides"
      description="Free and premium guides."
      apiPath="/admin/guides"
      uiBasePath="/admin/guides"
      newLabel="New guide"
      rowTitle={(g) => g.title}
      emptyText="No guides yet."
      columns={[
        { header: "Title", render: (g) => <span className="font-medium text-zinc-900">{g.title}</span> },
        { header: "Country", render: (g) => <span className="text-zinc-500">{g.country}</span> },
        {
          header: "Price",
          render: (g) =>
            g.isPremium ? (
              <span className="text-zinc-700">
                {g.priceCurrency} {g.priceAmount}
              </span>
            ) : (
              <span className="text-emerald-600">Free</span>
            ),
        },
        { header: "Files", render: (g) => <span className="text-zinc-500">{g.fileCount}</span> },
        { header: "Status", render: (g) => <StatusPill published={g.isPublished} /> },
      ]}
    />
  );
}
