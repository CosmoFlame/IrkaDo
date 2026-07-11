import type { Metadata } from "next";
import Image from "next/image";
import { notFound } from "next/navigation";
import { getAllNewsSlugs, getNewsArticleBySlug } from "@/lib/api";
import { ContentLinks } from "@/components/ContentLinks";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";

export const dynamicParams = true;

export async function generateStaticParams() {
  const slugs = await getAllNewsSlugs();
  return slugs.map((slug) => ({ slug }));
}

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const article = await getNewsArticleBySlug(slug, await getLocale());
  if (!article) return {};

  return {
    title: article.metaTitle || article.title,
    description: article.metaDescription || article.excerpt,
    openGraph: {
      title: article.metaTitle || article.title,
      description: article.metaDescription || article.excerpt,
      images: article.ogImageUrl ? [article.ogImageUrl] : undefined,
      type: "article",
    },
  };
}

export default async function NewsArticlePage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const locale = await getLocale();
  const t = getDictionary(locale);
  const article = await getNewsArticleBySlug(slug, locale);

  if (!article) notFound();

  const articleJsonLd = {
    "@context": "https://schema.org",
    "@type": "Article",
    headline: article.title,
    description: article.metaDescription || article.excerpt,
    image: article.ogImageUrl || article.coverImageUrl || undefined,
    datePublished: article.publishedAt || undefined,
    author: { "@type": "Person", name: "Iryna Dolzhenko" },
    publisher: { "@type": "Organization", name: "Irka_do" },
  };

  return (
    <main className="mx-auto w-full max-w-3xl flex-1 px-6 py-24">
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{
          __html: JSON.stringify(articleJsonLd).replace(/</g, "\\u003c"),
        }}
      />
      <article>
        <div className="flex items-center gap-2 text-xs font-medium uppercase tracking-wide text-amber-600">
          {article.category && <span>{article.category}</span>}
          {article.publishedAt && (
            <>
              <span aria-hidden>·</span>
              <time dateTime={article.publishedAt}>
                {new Date(article.publishedAt).toLocaleDateString(locale, {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                })}
              </time>
            </>
          )}
          <span aria-hidden>·</span>
          <span>{article.readingTimeMinutes} {t.common.minRead}</span>
        </div>
        <h1 className="mt-4 text-4xl font-semibold tracking-tight text-zinc-900">
          {article.title}
        </h1>
        {article.coverImageUrl && (
          <div className="relative mt-8 aspect-[16/9] w-full overflow-hidden rounded-2xl bg-zinc-100">
            <Image
              src={article.coverImageUrl}
              alt={article.coverImageAlt || article.title}
              fill
              className="object-cover"
              priority
            />
          </div>
        )}
        {/* Article body is HTML authored in the admin (trusted single-admin CMS), so it's rendered
            as markup rather than escaped text — otherwise the tags show up literally. */}
        <div
          className="prose prose-zinc mt-10 max-w-none"
          dangerouslySetInnerHTML={{ __html: article.content }}
        />
        {article.links.length > 0 && (
          <div className="mt-10">
            <h2 className="text-lg font-semibold text-zinc-900">{t.common.links}</h2>
            <ContentLinks links={article.links} className="mt-3 flex flex-col gap-2" />
          </div>
        )}
        {article.tags.length > 0 && (
          <div className="mt-10 flex flex-wrap gap-2">
            {article.tags.map((tag) => (
              <span
                key={tag}
                className="rounded-full bg-zinc-100 px-3 py-1 text-xs font-medium text-zinc-600"
              >
                {tag}
              </span>
            ))}
          </div>
        )}
      </article>
    </main>
  );
}
