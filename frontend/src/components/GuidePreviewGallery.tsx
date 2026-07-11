"use client";

import Image from "next/image";
import { useCallback, useEffect, useState } from "react";
import type { ImageMeta } from "@/types/api";

type Labels = {
  previewAlt: string;
  previousImage: string;
  nextImage: string;
  closeGallery: string;
};

/**
 * Compact, overlapping "photo pile" of guide preview images. Each thumbnail is clickable and
 * opens a full-screen lightbox with prev/next navigation (buttons + arrow keys, Esc to close).
 */
export function GuidePreviewGallery({
  images,
  labels,
}: {
  images: ImageMeta[];
  labels: Labels;
}) {
  const [openIndex, setOpenIndex] = useState<number | null>(null);
  const isOpen = openIndex !== null;

  const close = useCallback(() => setOpenIndex(null), []);
  const showPrev = useCallback(
    () => setOpenIndex((i) => (i === null ? i : (i - 1 + images.length) % images.length)),
    [images.length],
  );
  const showNext = useCallback(
    () => setOpenIndex((i) => (i === null ? i : (i + 1) % images.length)),
    [images.length],
  );

  // Keyboard navigation + lock body scroll while the lightbox is open.
  useEffect(() => {
    if (!isOpen) return;

    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") close();
      else if (e.key === "ArrowLeft") showPrev();
      else if (e.key === "ArrowRight") showNext();
    };
    document.addEventListener("keydown", onKey);

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.removeEventListener("keydown", onKey);
      document.body.style.overflow = previousOverflow;
    };
  }, [isOpen, close, showPrev, showNext]);

  if (images.length === 0) return null;

  const active = openIndex !== null ? images[openIndex] : null;

  return (
    <>
      {/* Overlapping thumbnails — like a fanned-out photo/card catalog. */}
      <div className="mt-10 flex flex-wrap items-center pl-5">
        {images.map((image, i) => (
          <button
            key={image.url}
            type="button"
            onClick={() => setOpenIndex(i)}
            style={{ rotate: `${(i % 2 === 0 ? -1 : 1) * (2 + (i % 3))}deg` }}
            className="group relative -ml-5 mb-2 h-28 w-28 shrink-0 overflow-hidden rounded-xl bg-zinc-100 shadow-md ring-4 ring-white transition duration-200 hover:z-10 hover:-translate-y-1 hover:rotate-0 hover:shadow-xl focus-visible:z-10 sm:h-32 sm:w-32"
          >
            <Image
              src={image.url}
              alt={image.alt || labels.previewAlt}
              fill
              sizes="128px"
              className="object-cover"
            />
          </button>
        ))}
      </div>

      {/* Lightbox */}
      {active && (
        <div
          role="dialog"
          aria-modal="true"
          onClick={close}
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/90 p-4 sm:p-8"
        >
          <button
            type="button"
            onClick={close}
            aria-label={labels.closeGallery}
            className="absolute right-4 top-4 inline-flex h-11 w-11 items-center justify-center rounded-full bg-white/10 text-white transition hover:bg-white/20"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" className="h-6 w-6" aria-hidden>
              <path d="M6 6l12 12M18 6L6 18" />
            </svg>
          </button>

          {images.length > 1 && (
            <>
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  showPrev();
                }}
                aria-label={labels.previousImage}
                className="absolute left-3 inline-flex h-11 w-11 items-center justify-center rounded-full bg-white/10 text-white transition hover:bg-white/20 sm:left-6"
              >
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="h-6 w-6" aria-hidden>
                  <path d="M15 18l-6-6 6-6" />
                </svg>
              </button>
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  showNext();
                }}
                aria-label={labels.nextImage}
                className="absolute right-3 inline-flex h-11 w-11 items-center justify-center rounded-full bg-white/10 text-white transition hover:bg-white/20 sm:right-6"
              >
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="h-6 w-6" aria-hidden>
                  <path d="M9 18l6-6-6-6" />
                </svg>
              </button>
            </>
          )}

          {/* Stop propagation so clicking the image itself doesn't close the lightbox. */}
          <div
            onClick={(e) => e.stopPropagation()}
            className="relative h-full max-h-[85vh] w-full max-w-4xl"
          >
            <Image
              src={active.url}
              alt={active.alt || labels.previewAlt}
              fill
              sizes="(min-width: 896px) 896px, 100vw"
              className="object-contain"
              priority
            />
          </div>

          {images.length > 1 && (
            <p className="absolute bottom-4 left-1/2 -translate-x-1/2 rounded-full bg-white/10 px-3 py-1 text-sm text-white">
              {openIndex! + 1} / {images.length}
            </p>
          )}
        </div>
      )}
    </>
  );
}
