"use client";

import { ResourceList } from "@/components/admin/ResourceList";
import type { AdminSocialLink } from "@/types/admin";

export default function SocialLinksListPage() {
  return (
    <ResourceList<AdminSocialLink>
      title="Social Links"
      description="Platforms shown in the site's social section."
      apiPath="/admin/social-links"
      uiBasePath="/admin/social-links"
      newLabel="New link"
      rowTitle={(s) => s.platform}
      emptyText="No social links yet."
      columns={[
        { header: "Platform", render: (s) => <span className="font-medium text-zinc-900">{s.platform}</span> },
        {
          header: "URL",
          render: (s) => (
            <span className="text-zinc-500">{s.url.length > 40 ? s.url.slice(0, 40) + "…" : s.url}</span>
          ),
        },
        { header: "Order", render: (s) => <span className="text-zinc-500">{s.displayOrder}</span> },
      ]}
    />
  );
}
