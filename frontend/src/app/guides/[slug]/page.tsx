import type { Metadata } from "next";
import Image from "next/image";
import { notFound } from "next/navigation";
import { getAllGuideSlugs, getTravelGuideBySlug } from "@/lib/api";
import { GuidePurchaseActions } from "@/components/GuidePurchaseActions";
import { GuidePreviewGallery } from "@/components/GuidePreviewGallery";
import { BackButton } from "@/components/BackButton";
import { ContentLinks } from "@/components/ContentLinks";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";

export const dynamicParams = true;

export async function generateStaticParams() {
  const slugs = await getAllGuideSlugs();
  return slugs.map((slug) => ({ slug }));
}

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const guide = await getTravelGuideBySlug(slug, await getLocale());
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
  const locale = await getLocale();
  const t = getDictionary(locale);
  const guide = await getTravelGuideBySlug(slug, locale);

  if (!guide) notFound();

  const guideJsonLd = {
    "@context": "https://schema.org",
    "@type": "Product",
    name: guide.title,
    description: guide.metaDescription || guide.description,
    image: guide.ogImageUrl || guide.coverImageUrl || undefined,
    brand: { "@type": "Organization", name: "Irka_do" },
    offers: {
      "@type": "Offer",
      price: guide.isPremium ? guide.priceAmount?.toString() : "0",
      priceCurrency: guide.priceCurrency,
      availability: "https://schema.org/InStock",
    },
  };

  return (
    <main className="mx-auto w-full max-w-4xl flex-1 px-6 py-24">
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{
          __html: JSON.stringify(guideJsonLd).replace(/</g, "\\u003c"),
        }}
      />
      <div className="relative">
        <BackButton label={t.guideDetail.back} className="absolute left-3 top-3 z-10" />
        {guide.coverImageUrl && (
          <div className="relative aspect-[16/9] w-full overflow-hidden rounded-2xl bg-zinc-100">
            <Image
              src={guide.coverImageUrl}
              alt={guide.coverImageAlt || guide.title}
              fill
              className="object-cover"
              priority
            />
          </div>
        )}
      </div>

      <p className="mt-8 text-sm font-medium uppercase tracking-[0.2em] text-amber-600">
        {guide.country}
        {guide.city ? ` · ${guide.city}` : ""}
      </p>
      <h1 className="mt-2 text-4xl font-semibold tracking-tight text-zinc-900">
        {guide.title}
      </h1>
      <p className="mt-2 text-sm text-zinc-500">
        {guide.durationDays} {guide.durationDays === 1 ? t.common.day : t.common.days}
        {guide.difficulty ? ` · ${guide.difficulty}` : ""}
        {guide.lastUpdatedAt &&
          ` · ${t.guideDetail.updated} ${new Date(guide.lastUpdatedAt).toLocaleDateString(locale)}`}
      </p>

      {/* Download/purchase action and outbound links sit right under the title, before the description. */}
      <GuidePurchaseActions
        slug={guide.slug}
        isPremium={guide.isPremium}
        priceAmount={guide.priceAmount}
        priceCurrency={guide.priceCurrency}
        hasFile={guide.hasFile}
        locale={locale}
      />

      {guide.links && guide.links.length > 0 && (
        <div className="mt-8">
          <h2 className="text-xl font-semibold text-zinc-900">{t.common.links}</h2>
          <ContentLinks links={guide.links} className="mt-3 flex flex-col gap-2" />
        </div>
      )}

      <p className="mt-8 text-lg leading-8 text-zinc-700">{guide.description}</p>

      {guide.whatsIncluded && (
        <div className="mt-8">
          <h2 className="text-xl font-semibold text-zinc-900">{t.guideDetail.whatsIncluded}</h2>
          <p className="mt-2 text-zinc-700">{guide.whatsIncluded}</p>
        </div>
      )}

      <GuidePreviewGallery
        images={guide.previewImages ?? []}
        labels={{
          previewAlt: t.guideDetail.previewAlt,
          previousImage: t.guideDetail.previousImage,
          nextImage: t.guideDetail.nextImage,
          closeGallery: t.guideDetail.closeGallery,
        }}
      />
    </main>
  );
}
