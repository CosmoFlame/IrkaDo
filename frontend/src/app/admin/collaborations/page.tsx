"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import { StatusPill } from "@/components/admin/ui";
import type { AdminCollaboration } from "@/types/admin";

export default function CollaborationsListPage() {
  return (
    <ResourceList<AdminCollaboration>
      title="Collaborations"
      description="Brand partnerships shown on the home page."
      apiPath="/admin/collaborations"
      uiBasePath="/admin/collaborations"
      newLabel="New collaboration"
      rowTitle={(c) => c.brandName}
      emptyText="No collaborations yet."
      columns={[
        { header: "Brand", render: (c) => <span className="font-medium text-zinc-900">{c.brandName}</span> },
        { header: "Order", render: (c) => <span className="text-zinc-500">{c.displayOrder}</span> },
        { header: "Status", render: (c) => <StatusPill published={c.isPublished} /> },
      ]}
    />
  );
}
