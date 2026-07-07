"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import { StatusPill } from "@/components/admin/ui";
import type { AdminHighlight } from "@/types/admin";

export default function HighlightsListPage() {
  return (
    <ResourceList<AdminHighlight>
      title="Travel Highlights"
      description="Featured destinations shown on the home page."
      apiPath="/admin/highlights"
      uiBasePath="/admin/highlights"
      newLabel="New highlight"
      rowTitle={(h) => h.destination}
      emptyText="No highlights yet."
      columns={[
        { header: "Destination", render: (h) => <span className="font-medium text-zinc-900">{h.destination}</span> },
        { header: "Caption", render: (h) => <span className="text-zinc-500">{h.caption}</span> },
        { header: "Order", render: (h) => <span className="text-zinc-500">{h.displayOrder}</span> },
        { header: "Status", render: (h) => <StatusPill published={h.isPublished} /> },
      ]}
    />
  );
}
