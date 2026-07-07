"use client";

import { NameSlugEditor } from "@/components/admin/NameSlugEditor";

export default function TagEditorPage({ params }: { params: Promise<{ id: string }> }) {
  return <NameSlugEditor params={params} apiPath="/admin/tags" uiBasePath="/admin/tags" singular="tag" />;
}
