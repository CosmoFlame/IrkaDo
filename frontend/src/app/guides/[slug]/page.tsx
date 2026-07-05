import type { Metadata } from "next";
import Image from "next/image";
import { notFound } from "next/navigation";
import { getTravelGuideBySlug } from "@/lib/api";
import { GuidePurchaseActions } from "@/components/GuidePurchaseActions";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const guide = await getTravelGuideBySlug(slug);
  if (!guide) return {};

  return {
    title: guide.metaTitle || guide.title,
    description: guide.metaDescription || guide.description,
    openGraph: {
      title: guide.metaTitle || guide.title,
      description: guide.metaDescription || guide.description,
      images: guide.ogImageUrl ? [guide.ogImageUrl] : undefined,
      type: "website",
    },
  };
}

export default async function GuideDetailPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const guide = await getTravelGuideBySlug(slug);

  if (!guide) notFound();

  return (
    <main className="mx-auto w-full max-w-4xl flex-1 px-6 py-24">
      {guide.coverImageUrl && (
        <div className="relative aspect-[16/9] w-full overflow-hidden rounded-2xl bg-zinc-100">
          <Image
            src={guide.coverImageUrl}
            alt={guide.title}
            fill
            className="object-cover"
            priority
          />
        </div>
      )}

      <p className="mt-8 text-sm font-medium uppercase tracking-[0.2em] text-amber-600">
        {guide.country}
        {guide.city ? ` · ${guide.city}` : ""} · {guide.continent}
      </p>
      <h1 className="mt-2 text-4xl font-semibold tracking-tight text-zinc-900">
        {guide.title}
      </h1>
      <p className="mt-2 text-sm text-zinc-500">
        {guide.durationDays} day{guide.durationDays === 1 ? "" : "s"}
        {guide.difficulty ? ` · ${guide.difficulty}` : ""}
        {guide.lastUpdatedAt &&
          ` · Updated ${new Date(guide.lastUpdatedAt).toLocaleDateString()}`}
      </p>

      <p className="mt-8 text-lg leading-8 text-zinc-700">{guide.description}</p>

      {guide.whatsIncluded && (
        <div className="mt-8">
          <h2 className="text-xl font-semibold text-zinc-900">What&apos;s Included</h2>
          <p className="mt-2 text-zinc-700">{guide.whatsIncluded}</p>
        </div>
      )}

      {guide.previewImageUrls.length > 0 && (
        <div className="mt-10 grid grid-cols-2 gap-4 sm:grid-cols-3">
          {guide.previewImageUrls.map((url) => (
            <div key={url} className="relative aspect-square overflow-hidden rounded-xl bg-zinc-100">
              <Image src={url} alt="Guide preview" fill className="object-cover" />
            </div>
          ))}
        </div>
      )}

      <GuidePurchaseActions
        slug={guide.slug}
        isPremium={guide.isPremium}
        priceAmount={guide.priceAmount}
        priceCurrency={guide.priceCurrency}
      />
    </main>
  );
}
