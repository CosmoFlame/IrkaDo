/* eslint-disable @next/next/no-img-element */
"use client";

import { useEffect, useRef, useState } from "react";
import { adminApi } from "@/lib/adminApi";
import { Button, Card, EmptyState, PageHeader, TextInput } from "@/components/admin/ui";
import type { AdminMedia } from "@/types/admin";

export default function MediaLibraryPage() {
  const [media, setMedia] = useState<AdminMedia[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const load = async () => {
    setLoading(true);
    try {
      setMedia(await adminApi.get<AdminMedia[]>("/admin/media"));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const upload = async (file: File) => {
    setUploading(true);
    try {
      const asset = await adminApi.upload<AdminMedia>("/admin/media", file);
      setMedia((m) => [asset, ...m]);
    } catch (err) {
      alert(err instanceof Error ? err.message : "Upload failed.");
    } finally {
      setUploading(false);
    }
  };

  const saveAlt = async (item: AdminMedia, altText: string) => {
    try {
      const updated = await adminApi.put<AdminMedia>(`/admin/media/${item.id}`, { altText: altText || null });
      setMedia((m) => m.map((x) => (x.id === item.id ? updated : x)));
    } catch (err) {
      alert(err instanceof Error ? err.message : "Save failed.");
    }
  };

  const remove = async (item: AdminMedia) => {
    if (!confirm("Delete this image?")) return;
    try {
      await adminApi.del(`/admin/media/${item.id}`);
      setMedia((m) => m.filter((x) => x.id !== item.id));
    } catch (err) {
      alert(err instanceof Error ? err.message : "Delete failed.");
    }
  };

  return (
    <div>
      <PageHeader
        title="Media"
        description="Images used across the site. Delete is blocked while an image is in use."
        actions={
          <>
            <input
              ref={inputRef}
              type="file"
              accept="image/*"
              hidden
              onChange={(e) => {
                const f = e.target.files?.[0];
                if (f) upload(f);
                e.target.value = "";
              }}
            />
            <Button disabled={uploading} onClick={() => inputRef.current?.click()}>
              {uploading ? "Uploading…" : "Upload image"}
            </Button>
          </>
        }
      />
      {loading && <p className="text-sm text-zinc-400">Loading…</p>}
      {error && <EmptyState>{error}</EmptyState>}
      {!loading && media.length === 0 && <EmptyState>No media yet.</EmptyState>}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {media.map((item) => (
          <MediaTile key={item.id} item={item} onSaveAlt={saveAlt} onRemove={remove} />
        ))}
      </div>
    </div>
  );
}

function MediaTile({
  item,
  onSaveAlt,
  onRemove,
}: {
  item: AdminMedia;
  onSaveAlt: (item: AdminMedia, alt: string) => void;
  onRemove: (item: AdminMedia) => void;
}) {
  const [alt, setAlt] = useState(item.altText ?? "");
  const dirty = alt !== (item.altText ?? "");

  return (
    <Card className="space-y-3 p-4">
      <div className="aspect-video overflow-hidden rounded-lg bg-zinc-100">
        <img src={item.url} alt={item.altText ?? ""} className="h-full w-full object-cover" />
      </div>
      <TextInput placeholder="Alt text" value={alt} onChange={(e) => setAlt(e.target.value)} />
      <div className="flex items-center justify-between">
        <button
          onClick={() => onSaveAlt(item, alt)}
          disabled={!dirty}
          className="text-sm font-medium text-zinc-700 hover:text-zinc-900 disabled:opacity-40"
        >
          Save alt
        </button>
        <button onClick={() => onRemove(item)} className="text-sm font-medium text-red-600 hover:text-red-700">
          Delete
        </button>
      </div>
    </Card>
  );
}
