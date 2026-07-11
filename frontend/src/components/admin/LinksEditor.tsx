"use client";

import type { AdminLink } from "@/types/admin";
import { Button, TextInput } from "@/components/admin/ui";

/**
 * Inline editor for a content's outbound links (news / guides / collaborations). Each row has a
 * URL plus an optional bilingual title. Order follows the row order and is stamped on save.
 */
export function LinksEditor({
  value,
  onChange,
}: {
  value: AdminLink[];
  onChange: (links: AdminLink[]) => void;
}) {
  const update = (index: number, patch: Partial<AdminLink>) =>
    onChange(value.map((link, i) => (i === index ? { ...link, ...patch } : link)));

  const add = () =>
    onChange([...value, { url: "", title: null, titleEn: null, displayOrder: value.length }]);

  const remove = (index: number) =>
    onChange(value.filter((_, i) => i !== index).map((link, i) => ({ ...link, displayOrder: i })));

  return (
    <div className="space-y-3">
      {value.length === 0 && <p className="text-sm text-zinc-400">No links added yet.</p>}
      {value.map((link, i) => (
        <div key={i} className="space-y-2 rounded-lg border border-zinc-200 p-3">
          <div className="flex items-center gap-2">
            <TextInput
              value={link.url}
              onChange={(e) => update(i, { url: e.target.value })}
              placeholder="https://instagram.com/p/…"
            />
            <button
              type="button"
              onClick={() => remove(i)}
              className="shrink-0 text-sm font-medium text-red-600 hover:text-red-700"
            >
              Remove
            </button>
          </div>
          <div className="grid gap-2 sm:grid-cols-2">
            <TextInput
              value={link.title ?? ""}
              onChange={(e) => update(i, { title: e.target.value || null })}
              placeholder="Title (UA)"
            />
            <TextInput
              value={link.titleEn ?? ""}
              onChange={(e) => update(i, { titleEn: e.target.value || null })}
              placeholder="Title (EN)"
            />
          </div>
        </div>
      ))}
      <Button type="button" variant="secondary" onClick={add}>
        Add link
      </Button>
    </div>
  );
}
