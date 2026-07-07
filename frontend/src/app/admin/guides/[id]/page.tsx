"use client";

import { use, useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker, MultiMediaPicker } from "@/components/admin/MediaPicker";
import {
  BilingualField,
  Button,
  Card,
  Checkbox,
  ErrorText,
  Field,
  PageHeader,
  Select,
  TextInput,
} from "@/components/admin/ui";
import type { AdminGuideDetail, AdminGuideFile, AdminGuideUpsert, GuideDifficulty } from "@/types/admin";

const DIFFICULTIES: GuideDifficulty[] = ["Easy", "Moderate", "Challenging"];

const EMPTY: AdminGuideUpsert = {
  title: "",
  titleEn: null,
  slug: "",
  country: "",
  countryEn: null,
  city: null,
  cityEn: null,
  continent: "",
  continentEn: null,
  description: "",
  descriptionEn: null,
  whatsIncluded: null,
  whatsIncludedEn: null,
  durationDays: 1,
  difficulty: null,
  isPremium: false,
  priceAmount: null,
  priceCurrency: "USD",
  isPublished: false,
  isFeatured: false,
  coverImageId: "",
  previewImageIds: [],
  metaTitle: null,
  metaTitleEn: null,
  metaDescription: null,
  metaDescriptionEn: null,
  ogImageUrl: null,
};

function GuideFilesManager({ guideId, initial }: { guideId: string; initial: AdminGuideFile[] }) {
  const [files, setFiles] = useState<AdminGuideFile[]>(initial);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const upload = async (file: File) => {
    setUploading(true);
    setError(null);
    try {
      const uploaded = await adminApi.upload<AdminGuideFile>(`/admin/guides/${guideId}/files`, file);
      setFiles((f) => [...f, uploaded]);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Upload failed.");
    } finally {
      setUploading(false);
    }
  };

  const remove = async (fileId: string) => {
    if (!confirm("Remove this file?")) return;
    await adminApi.del(`/admin/guides/${guideId}/files/${fileId}`);
    setFiles((f) => f.filter((x) => x.id !== fileId));
  };

  return (
    <div className="space-y-3">
      {files.length === 0 && <p className="text-sm text-zinc-400">No downloadable files attached yet.</p>}
      {files.map((f) => (
        <div key={f.id} className="flex items-center justify-between rounded-lg border border-zinc-200 px-3 py-2">
          <span className="text-sm text-zinc-700">
            {f.fileName} <span className="text-zinc-400">({Math.round(f.sizeBytes / 1024)} KB)</span>
          </span>
          <button type="button" onClick={() => remove(f.id)} className="text-sm font-medium text-red-600 hover:text-red-700">
            Remove
          </button>
        </div>
      ))}
      <input
        ref={inputRef}
        type="file"
        hidden
        onChange={(e) => {
          const f = e.target.files?.[0];
          if (f) upload(f);
          e.target.value = "";
        }}
      />
      <Button type="button" variant="secondary" disabled={uploading} onClick={() => inputRef.current?.click()}>
        {uploading ? "Uploading…" : "Upload file"}
      </Button>
      {error && <p className="text-sm text-red-600">{error}</p>}
    </div>
  );
}

export default function GuideEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminGuideUpsert>(EMPTY);
  const [files, setFiles] = useState<AdminGuideFile[]>([]);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<AdminGuideDetail>(`/admin/guides/${id}`)
      .then((g) => {
        setForm({
          title: g.title,
          titleEn: g.titleEn,
          slug: g.slug,
          country: g.country,
          countryEn: g.countryEn,
          city: g.city,
          cityEn: g.cityEn,
          continent: g.continent,
          continentEn: g.continentEn,
          description: g.description,
          descriptionEn: g.descriptionEn,
          whatsIncluded: g.whatsIncluded,
          whatsIncludedEn: g.whatsIncludedEn,
          durationDays: g.durationDays,
          difficulty: g.difficulty,
          isPremium: g.isPremium,
          priceAmount: g.priceAmount,
          priceCurrency: g.priceCurrency,
          isPublished: g.isPublished,
          isFeatured: g.isFeatured,
          coverImageId: g.coverImageId,
          previewImageIds: g.previewImageIds,
          metaTitle: g.metaTitle,
          metaTitleEn: g.metaTitleEn,
          metaDescription: g.metaDescription,
          metaDescriptionEn: g.metaDescriptionEn,
          ogImageUrl: g.ogImageUrl,
        });
        setFiles(g.files);
      })
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew]);

  const set = <K extends keyof AdminGuideUpsert>(k: K, v: AdminGuideUpsert[K]) => setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.coverImageId) {
      setError("Please choose a cover image.");
      return;
    }
    if (form.isPremium && (!form.priceAmount || form.priceAmount <= 0)) {
      setError("Premium guides need a price greater than zero.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      if (isNew) {
        const created = await adminApi.post<AdminGuideDetail>("/admin/guides", form);
        // Send the user back into edit mode so they can attach downloadable files.
        router.push(`/admin/guides/${created.id}`);
      } else {
        await adminApi.put(`/admin/guides/${id}`, form);
        router.push("/admin/guides");
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New guide" : "Edit guide"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/guides")}>
              Cancel
            </Button>
            <Button type="submit" disabled={saving}>
              {saving ? "Saving…" : "Save"}
            </Button>
          </>
        }
      />
      <div className="space-y-6">
        <Card className="space-y-4">
          <BilingualField
            label="Title"
            uk={form.title}
            en={form.titleEn}
            onUk={(v) => set("title", v)}
            onEn={(v) => set("titleEn", v)}
            required
          />
          <Field label="Slug">
            <TextInput value={form.slug} onChange={(e) => set("slug", e.target.value)} required />
          </Field>
          <BilingualField
            label="Country"
            uk={form.country}
            en={form.countryEn}
            onUk={(v) => set("country", v)}
            onEn={(v) => set("countryEn", v)}
            required
          />
          <BilingualField
            label="City"
            uk={form.city ?? ""}
            en={form.cityEn}
            onUk={(v) => set("city", v || null)}
            onEn={(v) => set("cityEn", v)}
          />
          <BilingualField
            label="Continent"
            uk={form.continent}
            en={form.continentEn}
            onUk={(v) => set("continent", v)}
            onEn={(v) => set("continentEn", v)}
            required
          />
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
            label="What's included"
            multiline
            uk={form.whatsIncluded ?? ""}
            en={form.whatsIncludedEn}
            onUk={(v) => set("whatsIncluded", v || null)}
            onEn={(v) => set("whatsIncludedEn", v)}
          />
          <div className="grid grid-cols-2 gap-4">
            <Field label="Duration (days)">
              <TextInput
                type="number"
                min={1}
                value={form.durationDays}
                onChange={(e) => set("durationDays", Number(e.target.value))}
              />
            </Field>
            <Field label="Difficulty">
              <Select
                value={form.difficulty ?? ""}
                onChange={(e) => set("difficulty", (e.target.value || null) as GuideDifficulty | null)}
              >
                <option value="">Not specified</option>
                {DIFFICULTIES.map((d) => (
                  <option key={d} value={d}>
                    {d}
                  </option>
                ))}
              </Select>
            </Field>
          </div>
        </Card>

        <Card className="space-y-4">
          <Checkbox label="Premium (paid) guide" checked={form.isPremium} onChange={(v) => set("isPremium", v)} />
          {form.isPremium && (
            <div className="grid grid-cols-2 gap-4">
              <Field label="Price">
                <TextInput
                  type="number"
                  min={0}
                  step="0.01"
                  value={form.priceAmount ?? ""}
                  onChange={(e) => set("priceAmount", e.target.value ? Number(e.target.value) : null)}
                />
              </Field>
              <Field label="Currency">
                <TextInput value={form.priceCurrency} onChange={(e) => set("priceCurrency", e.target.value)} />
              </Field>
            </div>
          )}
          <div className="flex gap-6">
            <Checkbox label="Published" checked={form.isPublished} onChange={(v) => set("isPublished", v)} />
            <Checkbox label="Featured on home" checked={form.isFeatured} onChange={(v) => set("isFeatured", v)} />
          </div>
        </Card>

        <Card className="space-y-4">
          <Field label="Cover image">
            <MediaPicker value={form.coverImageId || null} onChange={(v) => set("coverImageId", v ?? "")} label="cover" />
          </Field>
          <Field label="Preview images">
            <MultiMediaPicker value={form.previewImageIds} onChange={(ids) => set("previewImageIds", ids)} />
          </Field>
        </Card>

        <Card className="space-y-3">
          <p className="text-sm font-semibold text-zinc-700">Downloadable files</p>
          {isNew ? (
            <p className="text-sm text-zinc-400">Save the guide first, then you can attach downloadable files.</p>
          ) : (
            <GuideFilesManager guideId={id} initial={files} />
          )}
        </Card>

        <Card className="space-y-4">
          <p className="text-sm font-semibold text-zinc-700">SEO</p>
          <BilingualField
            label="Meta title"
            uk={form.metaTitle ?? ""}
            en={form.metaTitleEn}
            onUk={(v) => set("metaTitle", v || null)}
            onEn={(v) => set("metaTitleEn", v)}
          />
          <BilingualField
            label="Meta description"
            multiline
            uk={form.metaDescription ?? ""}
            en={form.metaDescriptionEn}
            onUk={(v) => set("metaDescription", v || null)}
            onEn={(v) => set("metaDescriptionEn", v)}
          />
          <Field label="OG image URL">
            <TextInput value={form.ogImageUrl ?? ""} onChange={(e) => set("ogImageUrl", e.target.value || null)} />
          </Field>
        </Card>

        <ErrorText>{error}</ErrorText>
      </div>
    </form>
  );
}
