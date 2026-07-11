"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { LinksEditor, cleanLinks } from "@/components/admin/LinksEditor";
import { BilingualField, Button, Card, Checkbox, ErrorText, Field, PageHeader, TextInput } from "@/components/admin/ui";
import type { AdminCollaboration, AdminCollaborationUpsert } from "@/types/admin";

const EMPTY: AdminCollaborationUpsert = {
  brandName: "",
  description: "",
  descriptionEn: null,
  testimonial: null,
  testimonialEn: null,
  displayOrder: 0,
  isPublished: true,
  coverImageId: "",
  links: [],
};

export default function CollaborationEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminCollaborationUpsert>(EMPTY);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<AdminCollaboration>(`/admin/collaborations/${id}`)
      .then((c) =>
        setForm({
          brandName: c.brandName,
          description: c.description,
          descriptionEn: c.descriptionEn,
          testimonial: c.testimonial,
          testimonialEn: c.testimonialEn,
          displayOrder: c.displayOrder,
          isPublished: c.isPublished,
          coverImageId: c.coverImageId,
          links: c.links,
        }),
      )
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew]);

  const set = <K extends keyof AdminCollaborationUpsert>(k: K, v: AdminCollaborationUpsert[K]) =>
    setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.coverImageId) {
      setError("Please choose a cover image.");
      return;
    }
    setSaving(true);
    setError(null);
    const payload = { ...form, links: cleanLinks(form.links) };
    try {
      if (isNew) await adminApi.post("/admin/collaborations", payload);
      else await adminApi.put(`/admin/collaborations/${id}`, payload);
      router.push("/admin/collaborations");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New collaboration" : "Edit collaboration"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/collaborations")}>
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
          <Field label="Brand name">
            <TextInput value={form.brandName} onChange={(e) => set("brandName", e.target.value)} required />
          </Field>
          <Field label="Display order">
            <TextInput
              type="number"
              value={form.displayOrder}
              onChange={(e) => set("displayOrder", Number(e.target.value))}
            />
          </Field>
        </div>
        <BilingualField
          label="Description"
          multiline
          uk={form.description}
          en={form.descriptionEn}
          onUk={(v) => set("description", v)}
          onEn={(v) => set("descriptionEn", v)}
          required
        />
        <BilingualField
          label="Testimonial"
          multiline
          uk={form.testimonial ?? ""}
          en={form.testimonialEn}
          onUk={(v) => set("testimonial", v || null)}
          onEn={(v) => set("testimonialEn", v)}
        />
        <Field label="Cover image">
          <MediaPicker value={form.coverImageId || null} onChange={(v) => set("coverImageId", v ?? "")} label="cover" />
        </Field>
        <Field label="Links">
          <LinksEditor value={form.links} onChange={(links) => set("links", links)} />
        </Field>
        <Checkbox label="Published" checked={form.isPublished} onChange={(v) => set("isPublished", v)} />
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
