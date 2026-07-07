/* eslint-disable @next/next/no-img-element */
"use client";

import { useEffect, useRef, useState } from "react";
import { adminApi } from "@/lib/adminApi";
import type { AdminMedia } from "@/types/admin";
import { Button, cx } from "@/components/admin/ui";

// Shared gallery cache so multiple pickers on one page don't each refetch.
let cache: AdminMedia[] | null = null;

function useMedia() {
  const [media, setMedia] = useState<AdminMedia[]>(cache ?? []);
  const [loading, setLoading] = useState(cache === null);

  // Fetch once into the shared module cache; if it's already populated the initial state
  // above covers it, so the effect does no synchronous setState.
  useEffect(() => {
    if (cache) return;
    let active = true;
    adminApi
      .get<AdminMedia[]>("/admin/media")
      .then((items) => {
        cache = items;
        if (active) setMedia(items);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, []);

  const add = (asset: AdminMedia) => {
    cache = [asset, ...(cache ?? [])];
    setMedia(cache);
  };

  return { media, loading, add };
}

function Gallery({
  selectedIds,
  onToggle,
}: {
  selectedIds: string[];
  onToggle: (id: string) => void;
}) {
  const { media, loading, add } = useMedia();
  const fileRef = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleUpload = async (file: File) => {
    setUploading(true);
    setError(null);
    try {
      const asset = await adminApi.upload<AdminMedia>("/admin/media", file);
      add(asset);
      onToggle(asset.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Upload failed.");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="mt-2 rounded-xl border border-zinc-200 bg-zinc-50 p-3">
      <div className="mb-3 flex items-center justify-between">
        <span className="text-xs font-medium text-zinc-500">Choose from library or upload</span>
        <input
          ref={fileRef}
          type="file"
          accept="image/*"
          hidden
          onChange={(e) => {
            const f = e.target.files?.[0];
            if (f) handleUpload(f);
            e.target.value = "";
          }}
        />
        <Button
          type="button"
          variant="secondary"
          disabled={uploading}
          onClick={() => fileRef.current?.click()}
        >
          {uploading ? "Uploading…" : "Upload new"}
        </Button>
      </div>
      {error && <p className="mb-2 text-xs text-red-600">{error}</p>}
      {loading ? (
        <p className="text-xs text-zinc-400">Loading library…</p>
      ) : media.length === 0 ? (
        <p className="text-xs text-zinc-400">No images yet — upload one above.</p>
      ) : (
        <div className="grid max-h-64 grid-cols-4 gap-2 overflow-y-auto sm:grid-cols-6">
          {media.map((m) => {
            const selected = selectedIds.includes(m.id);
            return (
              <button
                key={m.id}
                type="button"
                onClick={() => onToggle(m.id)}
                title={m.altText ?? undefined}
                className={cx(
                  "relative aspect-square overflow-hidden rounded-lg border-2",
                  selected ? "border-zinc-900" : "border-transparent hover:border-zinc-300",
                )}
              >
                <img src={m.url} alt={m.altText ?? ""} className="h-full w-full object-cover" />
                {selected && (
                  <span className="absolute right-1 top-1 flex h-5 w-5 items-center justify-center rounded-full bg-zinc-900 text-[10px] font-bold text-white">
                    ✓
                  </span>
                )}
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

export function MediaPicker({
  value,
  onChange,
  label = "Image",
}: {
  value: string | null;
  onChange: (id: string | null) => void;
  label?: string;
}) {
  const [open, setOpen] = useState(false);
  const { media } = useMedia();
  const selected = media.find((m) => m.id === value);

  return (
    <div>
      <div className="flex items-center gap-3">
        <div className="h-16 w-16 shrink-0 overflow-hidden rounded-lg border border-zinc-200 bg-zinc-100">
          {selected ? (
            <img src={selected.url} alt={selected.altText ?? ""} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-[10px] text-zinc-400">none</div>
          )}
        </div>
        <div className="flex gap-2">
          <Button type="button" variant="secondary" onClick={() => setOpen((o) => !o)}>
            {open ? "Close" : `Choose ${label.toLowerCase()}`}
          </Button>
          {value && (
            <Button type="button" variant="danger" onClick={() => onChange(null)}>
              Clear
            </Button>
          )}
        </div>
      </div>
      {open && (
        <Gallery
          selectedIds={value ? [value] : []}
          onToggle={(id) => {
            onChange(id === value ? null : id);
            setOpen(false);
          }}
        />
      )}
    </div>
  );
}

export function MultiMediaPicker({
  value,
  onChange,
}: {
  value: string[];
  onChange: (ids: string[]) => void;
}) {
  const [open, setOpen] = useState(false);
  const { media } = useMedia();
  const selected = media.filter((m) => value.includes(m.id));

  const toggle = (id: string) =>
    onChange(value.includes(id) ? value.filter((v) => v !== id) : [...value, id]);

  return (
    <div>
      <div className="flex flex-wrap items-center gap-2">
        {selected.map((m) => (
          <div key={m.id} className="h-14 w-14 overflow-hidden rounded-lg border border-zinc-200">
            <img src={m.url} alt={m.altText ?? ""} className="h-full w-full object-cover" />
          </div>
        ))}
        <Button type="button" variant="secondary" onClick={() => setOpen((o) => !o)}>
          {open ? "Close" : "Edit images"}
        </Button>
      </div>
      {open && <Gallery selectedIds={value} onToggle={toggle} />}
    </div>
  );
}
