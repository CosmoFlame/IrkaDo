"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { BilingualField, Button, Card, ErrorText, Field, PageHeader, Select, TextInput } from "@/components/admin/ui";
import type { AdminSocialLink, AdminSocialLinkUpsert, SocialPlatform } from "@/types/admin";

const PLATFORMS: SocialPlatform[] = ["Instagram", "TikTok", "YouTube", "Telegram", "Threads"];

const EMPTY: AdminSocialLinkUpsert = {
  platform: "Instagram",
  url: "",
  description: null,
  descriptionEn: null,
  followerCount: null,
  displayOrder: 0,
};

export default function SocialLinkEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminSocialLinkUpsert>(EMPTY);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<AdminSocialLink>(`/admin/social-links/${id}`)
      .then((s) =>
        setForm({
          platform: s.platform,
          url: s.url,
          description: s.description,
          descriptionEn: s.descriptionEn,
          followerCount: s.followerCount,
          displayOrder: s.displayOrder,
        }),
      )
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew]);

  const set = <K extends keyof AdminSocialLinkUpsert>(k: K, v: AdminSocialLinkUpsert[K]) =>
    setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      if (isNew) await adminApi.post("/admin/social-links", form);
      else await adminApi.put(`/admin/social-links/${id}`, form);
      router.push("/admin/social-links");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New social link" : "Edit social link"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/social-links")}>
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
          <Field label="Platform">
            <Select value={form.platform} onChange={(e) => set("platform", e.target.value as SocialPlatform)}>
              {PLATFORMS.map((p) => (
                <option key={p} value={p}>
                  {p}
                </option>
              ))}
            </Select>
          </Field>
          <Field label="Display order">
            <TextInput
              type="number"
              value={form.displayOrder}
              onChange={(e) => set("displayOrder", Number(e.target.value))}
            />
          </Field>
        </div>
        <Field label="URL">
          <TextInput type="url" value={form.url} onChange={(e) => set("url", e.target.value)} required />
        </Field>
        <BilingualField
          label="Description"
          multiline
          uk={form.description ?? ""}
          en={form.descriptionEn}
          onUk={(v) => set("description", v || null)}
          onEn={(v) => set("descriptionEn", v)}
        />
        <Field label="Follower count" hint="Optional; reserved for future display.">
          <TextInput
            type="number"
            value={form.followerCount ?? ""}
            onChange={(e) => set("followerCount", e.target.value ? Number(e.target.value) : null)}
          />
        </Field>
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
