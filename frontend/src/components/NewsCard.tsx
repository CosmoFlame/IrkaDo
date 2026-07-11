import Image from "next/image";
import Link from "next/link";
import type { Locale } from "@/i18n/config";
import { getDictionary } from "@/i18n/dictionaries";
import type { NewsArticleSummary } from "@/types/api";

export function NewsCard({ article, locale }: { article: NewsArticleSummary; locale: Locale }) {
  const t = getDictionary(locale);
  return (
    <Link
      href={`/news/${article.slug}`}
      className="group block overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-zinc-900/5 transition hover:shadow-lg"
    >
      <div className="relative aspect-[16/10] w-full overflow-hidden bg-zinc-100">
        {article.coverImageUrl ? (
          <Image
            src={article.coverImageUrl}
            alt={article.coverImageAlt || article.title}
            fill
            className="object-cover transition duration-500 group-hover:scale-105"
            sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-zinc-400">
            {t.common.noImage}
          </div>
        )}
      </div>
      <div className="p-4 sm:p-5">
        <div className="flex flex-wrap items-center gap-x-2 gap-y-1 text-[11px] font-medium uppercase tracking-wide text-amber-600 sm:text-xs">
          {article.category && <span>{article.category}</span>}
          {article.publishedAt && (
            <>
              <span aria-hidden>·</span>
              <time dateTime={article.publishedAt}>
                {new Date(article.publishedAt).toLocaleDateString(locale, {
                  year: "numeric",
                  month: "short",
                  day: "numeric",
                })}
              </time>
            </>
          )}
          <span aria-hidden>·</span>
          <span>{article.readingTimeMinutes} {t.common.minRead}</span>
        </div>
        <h3 className="mt-2 text-base font-semibold text-zinc-900 sm:text-lg">{article.title}</h3>
        <p className="mt-2 line-clamp-3 text-sm text-zinc-600">{article.excerpt}</p>
      </div>
    </Link>
  );
}
