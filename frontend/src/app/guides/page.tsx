import type { Metadata } from "next";
import { getTravelGuides } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { GuideCard } from "@/components/GuideCard";
import { Reveal } from "@/components/motion/Reveal";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";
import { buildPageMetadata } from "@/lib/pageMetadata";

export async function generateMetadata(): Promise<Metadata> {
  const locale = await getLocale();
  const t = getDictionary(locale);
  return buildPageMetadata("guides", locale, {
    title: t.guidesPage.metaTitle,
    description: t.guidesPage.metaDescription,
  });
}

export default async function GuidesPage({
  searchParams,
}: {
  searchParams: Promise<{ country?: string; type?: string }>;
}) {
  const params = await searchParams;
  const locale = await getLocale();
  const t = getDictionary(locale);
  const guides =
    (await getTravelGuides({
      country: params.country,
      type: params.type === "free" || params.type === "premium" ? params.type : undefined,
      lang: locale,
    })) ?? [];

  return (
    <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-24">
      <Reveal>
        <SectionHeading eyebrow={t.guidesPage.eyebrow} title={t.guidesPage.title} />
      </Reveal>

      <form className="mt-10 flex flex-wrap justify-center gap-3" method="get">
        <label htmlFor="filter-country" className="sr-only">
          {t.guidesPage.filterByCountry}
        </label>
        <input
          id="filter-country"
          type="text"
          name="country"
          defaultValue={params.country}
          placeholder={t.guidesPage.filterCountry}
          className="rounded-full border border-zinc-300 px-4 py-2 text-sm"
        />
        <label htmlFor="filter-type" className="sr-only">
          {t.guidesPage.filterByPrice}
        </label>
        <select
          id="filter-type"
          name="type"
          defaultValue={params.type ?? ""}
          className="rounded-full border border-zinc-300 px-4 py-2 text-sm"
        >
          <option value="">{t.guidesPage.all}</option>
          <option value="free">{t.guidesPage.free}</option>
          <option value="premium">{t.guidesPage.premium}</option>
        </select>
        <button
          type="submit"
          className="rounded-full bg-zinc-900 px-5 py-2 text-sm font-semibold text-white transition hover:bg-zinc-700"
        >
          {t.guidesPage.filter}
        </button>
      </form>

      {guides.length > 0 ? (
        <div className="mt-12 grid grid-cols-1 gap-4 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
          {guides.map((guide, i) => (
            <Reveal as="div" key={guide.slug} delay={(i % 3) * 0.08}>
              <GuideCard guide={guide} locale={locale} />
            </Reveal>
          ))}
        </div>
      ) : (
        <p className="mt-8 text-center text-zinc-500">{t.guidesPage.empty}</p>
      )}
    </main>
  );
}
