"use client";

import { use, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi } from "@/lib/adminApi";
import { Button, Card, ErrorText, Field, PageHeader, TextInput } from "@/components/admin/ui";

interface NameSlug {
  id: string;
  name: string;
  slug: string;
}

/** Shared create/edit form for simple {name, slug} resources (categories, tags). */
export function NameSlugEditor({
  params,
  apiPath,
  uiBasePath,
  singular,
}: {
  params: Promise<{ id: string }>;
  apiPath: string;
  uiBasePath: string;
  singular: string;
}) {
  const { id } = use(params);
  const isNew = id === "new";
  const router = useRouter();

  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    adminApi
      .get<NameSlug>(`${apiPath}/${id}`)
      .then((r) => {
        setName(r.name);
        setSlug(r.slug);
      })
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load."))
      .finally(() => setLoading(false));
  }, [id, isNew, apiPath]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const body = { name, slug };
      if (isNew) await adminApi.post(apiPath, body);
      else await adminApi.put(`${apiPath}/${id}`, body);
      router.push(uiBasePath);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Save failed.");
      setSaving(false);
    }
  };

  if (loading) return <p className="text-sm text-zinc-400">Loading…</p>;

  return (
    <form onSubmit={handleSubmit}>
      <PageHeader
        title={`${isNew ? "New" : "Edit"} ${singular}`}
        actions={
          <>
            <Button type="button" variant="secondary" onClick={() => router.push(uiBasePath)}>
              Cancel
            </Button>
            <Button type="submit" disabled={saving}>
              {saving ? "Saving…" : "Save"}
            </Button>
          </>
        }
      />
      <Card className="space-y-4">
        <Field label="Name">
          <TextInput value={name} onChange={(e) => setName(e.target.value)} required />
        </Field>
        <Field label="Slug" hint="Lowercase, hyphenated identifier.">
          <TextInput value={slug} onChange={(e) => setSlug(e.target.value)} required />
        </Field>
        <ErrorText>{error}</ErrorText>
      </Card>
    </form>
  );
}
