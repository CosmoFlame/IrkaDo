"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { Button, Card, ErrorText, Field, PageHeader, TextArea, TextInput } from "@/components/admin/ui";
import type { AdminHomeSection, AdminHomeSectionUpdate } from "@/types/admin";

export default function HomeSectionEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const router = useRouter();

  const [section, setSection] = useState<AdminHomeSection | null>(null);
  const [form, setForm] = useState<AdminHomeSectionUpdate>({
    headline: "",
    body: "",
    contentJson: "{}",
    backgroundMediaId: null,
  });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    adminApi
      .get<AdminHomeSection>(`/admin/home-sections/${id}`)
      .then((s) => {
        setSection(s);
        setForm({
          headline: s.headline,
          body: s.body,
          contentJson: s.contentJson,
          backgroundMediaId: s.backgroundMediaId,
        });
      })
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id]);

  const set = <K extends keyof AdminHomeSectionUpdate>(k: K, v: AdminHomeSectionUpdate[K]) =>
    setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // Guard against saving invalid JSON into the content payload.
    try {
      JSON.parse(form.contentJson || "{}");
    } catch {
      setError("Content JSON is not valid JSON.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      await adminApi.put(`/admin/home-sections/${id}`, form);
      router.push("/admin/home-sections");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={section ? `Edit ${section.type} section` : "Edit section"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/home-sections")}>
              Cancel
            </Button>
            <Button type="submit" disabled={saving}>
              {saving ? "Saving…" : "Save"}
            </Button>
          </>
        }
      />
      <Card className="space-y-4">
        <Field label="Headline">
          <TextInput value={form.headline} onChange={(e) => set("headline", e.target.value)} required />
        </Field>
        <Field label="Body">
          <TextArea value={form.body} onChange={(e) => set("body", e.target.value)} className="min-h-40" />
        </Field>
        {section?.type === "Hero" && (
          <Field label="Background image">
            <MediaPicker
              value={form.backgroundMediaId}
              onChange={(v) => set("backgroundMediaId", v)}
              label="background"
            />
          </Field>
        )}
        <Field label="Content JSON" hint="Advanced: extra structured fields for this section.">
          <TextArea
            value={form.contentJson}
            onChange={(e) => set("contentJson", e.target.value)}
            className="font-mono text-xs"
          />
        </Field>
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
