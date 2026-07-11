"use client";

import { useAdminData } from "@/components/admin/useAdminData";
import { Card, EmptyState, PageHeader } from "@/components/admin/ui";
import type { AdminDashboard } from "@/types/admin";

function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <Card>
      <p className="text-sm text-zinc-500">{label}</p>
      <p className="mt-2 text-3xl font-semibold tracking-tight text-zinc-900">{value}</p>
    </Card>
  );
}

export default function AdminDashboardPage() {
  const { data, error, loading } = useAdminData<AdminDashboard>("/admin/dashboard");

  return (
    <div>
      <PageHeader title="Dashboard" description="Overview of your site content and sales." />
      {loading && <p className="text-sm text-zinc-400">Loading…</p>}
      {error && <EmptyState>{error}</EmptyState>}
      {data && (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
          <Stat label="News published" value={`${data.newsPublished} / ${data.newsTotal}`} />
          <Stat label="Guides published" value={`${data.guidesPublished} / ${data.guidesTotal}`} />
          <Stat label="Premium guides" value={data.premiumGuides} />
          <Stat label="Collaborations" value={data.collaborations} />
          <Stat label="Social links" value={data.socialLinks} />
          <Stat label="Media assets" value={data.mediaAssets} />
          <Stat label="Completed purchases" value={data.purchasesCompleted} />
          <Stat label="Revenue" value={`$${data.revenue.toFixed(2)}`} />
          <Stat label="Downloads" value={data.downloadsTotal} />
        </div>
      )}
    </div>
  );
}
