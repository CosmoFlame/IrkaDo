import type { Metadata } from "next";
import Link from "next/link";
import { getNewsArticles } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { NewsCard } from "@/components/NewsCard";
import { Reveal } from "@/components/motion/Reveal";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";

export async function generateMetadata(): Promise<Metadata> {
  const t = getDictionary(await getLocale());
  return {
    title: t.newsPage.metaTitle,
    description: t.newsPage.metaDescription,
  };
}

export default async function NewsPage({
  searchParams,
}: {
  searchParams: Promise<{ page?: string }>;
}) {
  const params = await searchParams;
  const locale = await getLocale();
  const t = getDictionary(locale);
  const page = Math.max(1, Number(params.page) || 1);
  const result = await getNewsArticles({ page, lang: locale });
  const articles = result?.items ?? [];
  const totalPages = result ? Math.max(1, Math.ceil(result.totalCount / result.pageSize)) : 1;

  return (
    <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-24">
      <Reveal>
        <SectionHeading eyebrow={t.newsPage.eyebrow} title={t.newsPage.title} />
      </Reveal>
      {articles.length > 0 ? (
        <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {articles.map((article, i) => (
            <Reveal as="div" key={article.slug} delay={(i % 3) * 0.08}>
              <NewsCard article={article} locale={locale} />
            </Reveal>
          ))}
        </div>
      ) : (
        <p className="mt-8 text-center text-zinc-500">{t.newsPage.empty}</p>
      )}

      {totalPages > 1 && (
        <div className="mt-12 flex items-center justify-center gap-4">
          <Link
            href={`/news?page=${page - 1}`}
            aria-disabled={page <= 1}
            className={`rounded-full border border-zinc-300 px-4 py-2 text-sm font-medium ${
              page <= 1
                ? "pointer-events-none opacity-40"
                : "text-zinc-700 hover:bg-zinc-100"
            }`}
          >
            {t.newsPage.previous}
          </Link>
          <span className="text-sm text-zinc-500">
            {t.newsPage.pageOf(page, totalPages)}
          </span>
          <Link
            href={`/news?page=${page + 1}`}
            aria-disabled={page >= totalPages}
            className={`rounded-full border border-zinc-300 px-4 py-2 text-sm font-medium ${
              page >= totalPages
                ? "pointer-events-none opacity-40"
                : "text-zinc-700 hover:bg-zinc-100"
            }`}
          >
            {t.newsPage.next}
          </Link>
        </div>
      )}
    </main>
  );
}
