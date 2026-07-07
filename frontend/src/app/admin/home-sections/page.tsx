"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import type { AdminHomeSection } from "@/types/admin";

export default function HomeSectionsListPage() {
  return (
    <ResourceList<AdminHomeSection>
      title="Home Sections"
      description="Editable copy for the Hero, About, and Contact sections."
      apiPath="/admin/home-sections"
      uiBasePath="/admin/home-sections"
      rowTitle={(s) => s.type}
      canCreate={false}
      canDelete={false}
      emptyText="No home sections found."
      columns={[
        { header: "Section", render: (s) => <span className="font-medium text-zinc-900">{s.type}</span> },
        { header: "Headline", render: (s) => <span className="text-zinc-500">{s.headline}</span> },
      ]}
    />
  );
}
