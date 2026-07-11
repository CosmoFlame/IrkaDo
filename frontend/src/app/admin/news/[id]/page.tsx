"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { MediaPicker } from "@/components/admin/MediaPicker";
import { LinksEditor, cleanLinks } from "@/components/admin/LinksEditor";
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
  cx,
} from "@/components/admin/ui";
import type { AdminCategory, AdminNewsDetail, AdminNewsUpsert, AdminTag } from "@/types/admin";

const EMPTY: AdminNewsUpsert = {
  title: "",
  titleEn: null,
  slug: "",
  excerpt: "",
  excerptEn: null,
  content: "",
  contentEn: null,
  readingTimeMinutes: 3,
  isPublished: false,
  coverImageId: "",
  categoryId: "",
  tagIds: [],
  links: [],
  metaTitle: null,
  metaTitleEn: null,
  metaDescription: null,
  metaDescriptionEn: null,
  ogImageUrl: null,
};

export default function NewsEditorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [form, setForm] = useState<AdminNewsUpsert>(EMPTY);
  const [categories, setCategories] = useState<AdminCategory[]>([]);
  const [tags, setTags] = useState<AdminTag[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const [cats, tgs] = await Promise.all([
          adminApi.get<AdminCategory[]>("/admin/categories"),
          adminApi.get<AdminTag[]>("/admin/tags"),
        ]);
        setCategories(cats);
        setTags(tgs);
        if (!isNew) {
          const detail = await adminApi.get<AdminNewsDetail>(`/admin/news/${id}`);
          setForm({
            title: detail.title,
            titleEn: detail.titleEn,
            slug: detail.slug,
            excerpt: detail.excerpt,
            excerptEn: detail.excerptEn,
            content: detail.content,
            contentEn: detail.contentEn,
            readingTimeMinutes: detail.readingTimeMinutes,
            isPublished: detail.isPublished,
            coverImageId: detail.coverImageId,
            categoryId: detail.categoryId,
            tagIds: detail.tagIds,
            links: detail.links,
            metaTitle: detail.metaTitle,
            metaTitleEn: detail.metaTitleEn,
            metaDescription: detail.metaDescription,
            metaDescriptionEn: detail.metaDescriptionEn,
            ogImageUrl: detail.ogImageUrl,
          });
        } else if (cats.length > 0) {
          setForm((f) => ({ ...f, categoryId: cats[0].id }));
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load.");
      } finally {
        setLoading(false);
      }
    })();
  }, [id, isNew]);

  const set = <K extends keyof AdminNewsUpsert>(key: K, value: AdminNewsUpsert[K]) =>
    setForm((f) => ({ ...f, [key]: value }));

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
      if (isNew) await adminApi.post("/admin/news", payload);
      else await adminApi.put(`/admin/news/${id}`, payload);
      router.push("/admin/news");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={isNew ? "New article" : "Edit article"}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push("/admin/news")}>
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
          <Field label="Slug" hint="Used in the URL, e.g. /news/my-article">
            <TextInput value={form.slug} onChange={(e) => set("slug", e.target.value)} required />
          </Field>
          <BilingualField
            label="Excerpt"
            multiline
            uk={form.excerpt}
            en={form.excerptEn}
            onUk={(v) => set("excerpt", v)}
            onEn={(v) => set("excerptEn", v)}
            required
          />
          <BilingualField
            label="Content"
            multiline
            textAreaClassName="min-h-60"
            uk={form.content}
            en={form.contentEn}
            onUk={(v) => set("content", v)}
            onEn={(v) => set("contentEn", v)}
            required
          />
        </Card>

        <Card className="space-y-4">
          <Field label="Cover image">
            <MediaPicker value={form.coverImageId || null} onChange={(v) => set("coverImageId", v ?? "")} label="cover" />
          </Field>
          <div className="grid grid-cols-2 gap-4">
            <Field label="Category">
              <Select value={form.categoryId} onChange={(e) => set("categoryId", e.target.value)} required>
                <option value="" disabled>
                  Select…
                </option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </Select>
            </Field>
            <Field label="Reading time (minutes)">
              <TextInput
                type="number"
                min={1}
                value={form.readingTimeMinutes}
                onChange={(e) => set("readingTimeMinutes", Number(e.target.value))}
              />
            </Field>
          </div>
          <Field label="Tags">
            <div className="flex flex-wrap gap-2">
              {tags.length === 0 && <span className="text-sm text-zinc-400">No tags defined.</span>}
              {tags.map((t) => {
                const active = form.tagIds.includes(t.id);
                return (
                  <button
                    type="button"
                    key={t.id}
                    onClick={() =>
                      set("tagIds", active ? form.tagIds.filter((x) => x !== t.id) : [...form.tagIds, t.id])
                    }
                    className={cx(
                      "rounded-full border px-3 py-1 text-sm",
                      active ? "border-zinc-900 bg-zinc-900 text-white" : "border-zinc-300 text-zinc-600",
                    )}
                  >
                    {t.name}
                  </button>
                );
              })}
            </div>
          </Field>
          <Checkbox label="Published" checked={form.isPublished} onChange={(v) => set("isPublished", v)} />
        </Card>

        <Card className="space-y-3">
          <p className="text-sm font-semibold text-zinc-700">Links</p>
          <LinksEditor value={form.links} onChange={(links) => set("links", links)} />
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
