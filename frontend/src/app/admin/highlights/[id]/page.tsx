"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { BilingualField, Button, Card, Checkbox, ErrorText, Field, PageHeader, TextInput } from "@/components/admin/ui";
import type { AdminHighlight, AdminHighlightUpsert } from "@/types/admin";

const EMPTY: AdminHighlightUpsert = {
  destination: "",
  destinationEn: null,
  caption: "",
  captionEn: null,
  displayOrder: 0,
  isPublished: true,
  imageId: "",
};

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
          destinationEn: h.destinationEn,
          caption: h.caption,
          captionEn: h.captionEn,
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
        <BilingualField
          label="Destination"
          uk={form.destination}
          en={form.destinationEn}
          onUk={(v) => set("destination", v)}
          onEn={(v) => set("destinationEn", v)}
          required
        />
        <Field label="Display order">
          <TextInput
            type="number"
            value={form.displayOrder}
            onChange={(e) => set("displayOrder", Number(e.target.value))}
          />
        </Field>
        <BilingualField
          label="Caption"
          multiline
          uk={form.caption}
          en={form.captionEn}
          onUk={(v) => set("caption", v)}
          onEn={(v) => set("captionEn", v)}
          required
        />
        <Field label="Image">
          <MediaPicker value={form.imageId || null} onChange={(v) => set("imageId", v ?? "")} />
        </Field>
        <Checkbox label="Published" checked={form.isPublished} onChange={(v) => set("isPublished", v)} />
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
