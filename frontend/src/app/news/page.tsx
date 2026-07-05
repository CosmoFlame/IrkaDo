import type { Metadata } from "next";
import { getNewsArticles } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { NewsCard } from "@/components/NewsCard";

export const metadata: Metadata = {
  title: "Travel News & Stories",
  description: "The latest travel news, stories, and updates from Irka_do.",
};

export default async function NewsPage() {
  const articles = (await getNewsArticles()) ?? [];

  return (
    <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-24">
      <SectionHeading eyebrow="Journal" title="Travel News" />
      {articles.length > 0 ? (
        <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {articles.map((article) => (
            <NewsCard key={article.slug} article={article} />
          ))}
        </div>
      ) : (
        <p className="mt-8 text-center text-zinc-500">
          No articles published yet — check back soon.
        </p>
      )}
    </main>
  );
}
