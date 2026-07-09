"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { BilingualField, Button, Card, ErrorText, Field, PageHeader, TextInput } from "@/components/admin/ui";
import type { AdminHomeSection, AdminHomeSectionUpdate } from "@/types/admin";

export default function HomeSectionEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const router = useRouter();

  const [section, setSection] = useState<AdminHomeSection | null>(null);
  const [form, setForm] = useState<AdminHomeSectionUpdate>({
    headline: "",
    headlineEn: null,
    body: "",
    bodyEn: null,
    contentJson: "{}",
    contentJsonEn: null,
    contactEmail: null,
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
          headlineEn: s.headlineEn,
          body: s.body,
          bodyEn: s.bodyEn,
          contentJson: s.contentJson,
          contentJsonEn: s.contentJsonEn,
          contactEmail: s.contactEmail,
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
    // Guard against saving invalid JSON into either content payload.
    try {
      JSON.parse(form.contentJson || "{}");
      if (form.contentJsonEn) JSON.parse(form.contentJsonEn);
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
        <BilingualField
          label="Headline"
          uk={form.headline}
          en={form.headlineEn}
          onUk={(v) => set("headline", v)}
          onEn={(v) => set("headlineEn", v)}
          required
        />
        <BilingualField
          label="Body"
          multiline
          textAreaClassName="min-h-40"
          uk={form.body}
          en={form.bodyEn}
          onUk={(v) => set("body", v)}
          onEn={(v) => set("bodyEn", v)}
        />
        {section?.type === "Hero" && (
          <Field label="Background image">
            <MediaPicker
              value={form.backgroundMediaId}
              onChange={(v) => set("backgroundMediaId", v)}
              label="background"
            />
          </Field>
        )}
        {section?.type === "Contact" && (
          <Field label="Contact email" hint="Shown as the mailto link in the Contact section.">
            <TextInput
              type="email"
              value={form.contactEmail ?? ""}
              onChange={(e) => set("contactEmail", e.target.value || null)}
              placeholder="hello@example.com"
            />
          </Field>
        )}
        <BilingualField
          label="Content JSON"
          hint="Advanced: extra structured fields for this section."
          multiline
          textAreaClassName="font-mono text-xs"
          uk={form.contentJson}
          en={form.contentJsonEn}
          onUk={(v) => set("contentJson", v)}
          onEn={(v) => set("contentJsonEn", v)}
        />
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
