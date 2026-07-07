"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { Button, Card, Checkbox, ErrorText, Field, PageHeader, TextArea, TextInput } from "@/components/admin/ui";
import type { AdminHighlight, AdminHighlightUpsert } from "@/types/admin";

const EMPTY: AdminHighlightUpsert = { destination: "", caption: "", displayOrder: 0, isPublished: true, imageId: "" };

export default function HighlightEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminHighlightUpsert>(EMPTY);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<AdminHighlight>(`/admin/highlights/${id}`)
      .then((h) =>
        setForm({
          destination: h.destination,
          caption: h.caption,
          displayOrder: h.displayOrder,
          isPublished: h.isPublished,
          imageId: h.imageId,
        }),
      )
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew]);

  const set = <K extends keyof AdminHighlightUpsert>(k: K, v: AdminHighlightUpsert[K]) =>
    setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.imageId) {
      setError("Please choose an image.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      if (isNew) await adminApi.post("/admin/highlights", form);
      else await adminApi.put(`/admin/highlights/${id}`, form);
      router.push("/admin/highlights");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New highlight" : "Edit highlight"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/highlights")}>
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
          <Field label="Destination">
            <TextInput value={form.destination} onChange={(e) => set("destination", e.target.value)} required />
          </Field>
          <Field label="Display order">
            <TextInput
              type="number"
              value={form.displayOrder}
              onChange={(e) => set("displayOrder", Number(e.target.value))}
            />
          </Field>
        </div>
        <Field label="Caption">
          <TextArea value={form.caption} onChange={(e) => set("caption", e.target.value)} required />
        </Field>
        <Field label="Image">
          <MediaPicker value={form.imageId || null} onChange={(v) => set("imageId", v ?? "")} />
        </Field>
        <Checkbox label="Published" checked={form.isPublished} onChange={(v) => set("isPublished", v)} />
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
