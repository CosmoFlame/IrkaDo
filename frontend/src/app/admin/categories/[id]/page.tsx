"use client";

import { NameSlugEditor } from "@/components/admin/NameSlugEditor";

export default function CategoryEditorPage({ params }: { params: Promise<{ id: string }> }) {
  return <NameSlugEditor params={params} apiPath="/admin/categories" uiBasePath="/admin/categories" singular="category" />;
}
