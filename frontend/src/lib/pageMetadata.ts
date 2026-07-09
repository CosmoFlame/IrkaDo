import type { Metadata } from "next";
import type { Locale } from "@/i18n/config";
import { getPageMeta } from "@/lib/api";

/**
 * Builds a page's <head> metadata from the editable `Page` CMS entry (by slug), falling back to the
 * given static copy when the page has no CMS entry yet. The resolved title is set as `absolute` so
 * the CMS value fully controls the tag rather than being wrapped by the root title template.
 */
export async function buildPageMetadata(
  slug: string,
  locale: Locale,
  fallback: { title: string; description: string },
): Promise<Metadata> {
  const page = await getPageMeta(slug, locale);
  const title = page?.metaTitle || page?.title || fallback.title;
  const description = page?.metaDescription || fallback.description;

  return {
    title: { absolute: title },
    description,
    openGraph: {
      title,
      description,
      images: page?.ogImageUrl ? [page.ogImageUrl] : undefined,
    },
  };
}
