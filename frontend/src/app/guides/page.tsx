import type { Metadata } from "next";
import { getTravelGuides } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { GuideCard } from "@/components/GuideCard";
import { Reveal } from "@/components/motion/Reveal";

export const metadata: Metadata = {
  title: "Travel Guides",
  description: "Free and premium travel guides curated by Irka_do.",
};

export default async function GuidesPage({
  searchParams,
}: {
  searchParams: Promise<{ country?: string; continent?: string; type?: string }>;
}) {
  const params = await searchParams;
  const guides =
    (await getTravelGuides({
      country: params.country,
      continent: params.continent,
      type: params.type === "free" || params.type === "premium" ? params.type : undefined,
    })) ?? [];

  return (
    <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-24">
      <Reveal>
        <SectionHeading eyebrow="Plan Your Trip" title="Travel Guides" />
      </Reveal>

      <form className="mt-10 flex flex-wrap justify-center gap-3" method="get">
        <label htmlFor="filter-country" className="sr-only">
          Filter by country
        </label>
        <input
          id="filter-country"
          type="text"
          name="country"
          defaultValue={params.country}
          placeholder="Country"
          className="rounded-full border border-zinc-300 px-4 py-2 text-sm"
        />
        <label htmlFor="filter-continent" className="sr-only">
          Filter by continent
        </label>
        <input
          id="filter-continent"
          type="text"
          name="continent"
          defaultValue={params.continent}
          placeholder="Continent"
          className="rounded-full border border-zinc-300 px-4 py-2 text-sm"
        />
        <label htmlFor="filter-type" className="sr-only">
          Filter by price
        </label>
        <select
          id="filter-type"
          name="type"
          defaultValue={params.type ?? ""}
          className="rounded-full border border-zinc-300 px-4 py-2 text-sm"
        >
          <option value="">All</option>
          <option value="free">Free</option>
          <option value="premium">Premium</option>
        </select>
        <button
          type="submit"
          className="rounded-full bg-zinc-900 px-5 py-2 text-sm font-semibold text-white transition hover:bg-zinc-700"
        >
          Filter
        </button>
      </form>

      {guides.length > 0 ? (
        <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {guides.map((guide, i) => (
            <Reveal as="div" key={guide.slug} delay={(i % 3) * 0.08}>
              <GuideCard guide={guide} />
            </Reveal>
          ))}
        </div>
      ) : (
        <p className="mt-8 text-center text-zinc-500">
          No guides match your filters yet.
        </p>
      )}
    </main>
  );
}
