"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { Button, Card, ErrorText, Field, PageHeader, TextArea, TextInput } from "@/components/admin/ui";
import type { AdminPage, AdminPageUpsert } from "@/types/admin";

const EMPTY: AdminPageUpsert = { slug: "", title: "", metaTitle: null, metaDescription: null, ogImageUrl: null };

export default function PageEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminPageUpsert>(EMPTY);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<AdminPage>(`/admin/pages/${id}`)
      .then((p) =>
        setForm({
          slug: p.slug,
          title: p.title,
          metaTitle: p.metaTitle,
          metaDescription: p.metaDescription,
          ogImageUrl: p.ogImageUrl,
        }),
      )
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew]);

  const set = <K extends keyof AdminPageUpsert>(k: K, v: AdminPageUpsert[K]) => setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      if (isNew) await adminApi.post("/admin/pages", form);
      else await adminApi.put(`/admin/pages/${id}`, form);
      router.push("/admin/pages");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New page" : "Edit page"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/pages")}>
              Cancel
            </Button>
            <Button type="submit" disabled={saving}>
              {saving ? "Saving…" : "Save"}
            </Button>
          </>
        }
      />
      <Card className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <Field label="Title">
            <TextInput value={form.title} onChange={(e) => set("title", e.target.value)} required />
          </Field>
          <Field label="Slug">
            <TextInput value={form.slug} onChange={(e) => set("slug", e.target.value)} required />
          </Field>
        </div>
        <Field label="Meta title">
          <TextInput value={form.metaTitle ?? ""} onChange={(e) => set("metaTitle", e.target.value || null)} />
        </Field>
        <Field label="Meta description">
          <TextArea
            value={form.metaDescription ?? ""}
            onChange={(e) => set("metaDescription", e.target.value || null)}
          />
        </Field>
        <Field label="OG image URL">
          <TextInput value={form.ogImageUrl ?? ""} onChange={(e) => set("ogImageUrl", e.target.value || null)} />
        </Field>
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
