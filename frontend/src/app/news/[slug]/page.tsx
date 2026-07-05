import type { Metadata } from "next";
import Image from "next/image";
import { notFound } from "next/navigation";
import { getNewsArticleBySlug } from "@/lib/api";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const article = await getNewsArticleBySlug(slug);
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
  const article = await getNewsArticleBySlug(slug);

  if (!article) notFound();

  return (
    <main className="mx-auto w-full max-w-3xl flex-1 px-6 py-24">
      <article>
        <div className="flex items-center gap-2 text-xs font-medium uppercase tracking-wide text-amber-600">
          {article.category && <span>{article.category}</span>}
          {article.publishedAt && (
            <>
              <span aria-hidden>·</span>
              <time dateTime={article.publishedAt}>
                {new Date(article.publishedAt).toLocaleDateString(undefined, {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                })}
              </time>
            </>
          )}
          <span aria-hidden>·</span>
          <span>{article.readingTimeMinutes} min read</span>
        </div>
        <h1 className="mt-4 text-4xl font-semibold tracking-tight text-zinc-900">
          {article.title}
        </h1>
        {article.coverImageUrl && (
          <div className="relative mt-8 aspect-[16/9] w-full overflow-hidden rounded-2xl bg-zinc-100">
            <Image
              src={article.coverImageUrl}
              alt={article.title}
              fill
              className="object-cover"
              priority
            />
          </div>
        )}
        <div className="prose prose-zinc mt-10 max-w-none">
          {article.content}
        </div>
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
