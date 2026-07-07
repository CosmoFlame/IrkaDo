import Image from "next/image";
import Link from "next/link";
import type { Locale } from "@/i18n/config";
import { getDictionary } from "@/i18n/dictionaries";
import type { TravelGuideSummary } from "@/types/api";

export function GuideCard({ guide, locale }: { guide: TravelGuideSummary; locale: Locale }) {
  const t = getDictionary(locale);
  return (
    <Link
      href={`/guides/${guide.slug}`}
      className="group block overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-zinc-900/5 transition hover:shadow-lg"
    >
      <div className="relative aspect-[4/3] w-full overflow-hidden bg-zinc-100">
        {guide.coverImageUrl ? (
          <Image
            src={guide.coverImageUrl}
            alt={guide.title}
            fill
            className="object-cover transition duration-500 group-hover:scale-105"
            sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-zinc-400">
            {t.common.noImage}
          </div>
        )}
        <span
          className={`absolute right-3 top-3 rounded-full px-3 py-1 text-xs font-semibold ${
            guide.isPremium
              ? "bg-amber-500 text-white"
              : "bg-emerald-500 text-white"
          }`}
        >
          {guide.isPremium ? `${guide.priceCurrency} ${guide.priceAmount}` : t.common.free}
        </span>
      </div>
      <div className="p-5">
        <p className="text-sm font-medium text-amber-600">
          {guide.country}
          {guide.city ? ` · ${guide.city}` : ""}
        </p>
        <h3 className="mt-1 text-lg font-semibold text-zinc-900">{guide.title}</h3>
        <p className="mt-1 text-sm text-zinc-500">
          {guide.durationDays} {guide.durationDays === 1 ? t.common.day : t.common.days}
          {guide.difficulty ? ` · ${guide.difficulty}` : ""}
        </p>
      </div>
    </Link>
  );
}
