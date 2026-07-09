import type { Metadata } from "next";
import Image from "next/image";
import Link from "next/link";
import { getHomePage } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { GuideCard } from "@/components/GuideCard";
import { NewsCard } from "@/components/NewsCard";
import { HeroContent } from "@/components/HeroContent";
import { Reveal } from "@/components/motion/Reveal";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";
import { buildPageMetadata } from "@/lib/pageMetadata";

export async function generateMetadata(): Promise<Metadata> {
  const locale = await getLocale();
  const t = getDictionary(locale);
  return buildPageMetadata("home", locale, {
    title: "Irka_do — Travel Creator",
    description: t.layout.metaDescription,
  });
}

export default async function Home() {
  const locale = await getLocale();
  const t = getDictionary(locale);
  const home = await getHomePage(locale);

  const hero = home?.hero;
  const about = home?.about;
  const contact = home?.contact;
  const highlights = home?.travelHighlights ?? [];
  const socialLinks = home?.socialLinks ?? [];
  const collaborations = home?.collaborations ?? [];
  const featuredGuides = home?.featuredGuides ?? [];
  const latestNews = home?.latestNews ?? [];

  return (
    <main className="flex flex-1 flex-col">
      {/* Hero */}
      <section className="relative flex min-h-[85vh] items-end overflow-hidden bg-zinc-900 text-white">
        {hero?.backgroundMediaUrl && (
          <Image
            src={hero.backgroundMediaUrl}
            alt=""
            fill
            priority
            className="object-cover opacity-70"
          />
        )}
        <HeroContent
          locale={locale}
          headline={hero?.headline || t.hero.fallbackHeadline}
          body={hero?.body || t.hero.fallbackBody}
        />
      </section>

      {/* About */}
      <section className="mx-auto w-full max-w-4xl px-6 py-24">
        <Reveal>
          <SectionHeading
            eyebrow={t.about.eyebrow}
            title={about?.headline || t.about.fallbackTitle}
            description={about?.body || t.about.fallbackBody}
          />
        </Reveal>
      </section>

      {/* Travel Highlights */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading eyebrow={t.highlights.eyebrow} title={t.highlights.title} />
          </Reveal>
          {highlights.length > 0 ? (
            <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {highlights.map((h, i) => (
                <Reveal
                  as="div"
                  key={h.destination}
                  delay={i * 0.08}
                  className="group relative aspect-[4/5] overflow-hidden rounded-2xl bg-zinc-100"
                >
                  {h.imageUrl && (
                    <Image
                      src={h.imageUrl}
                      alt={h.imageAlt || h.destination}
                      fill
                      className="object-cover transition duration-500 group-hover:scale-105"
                    />
                  )}
                  <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/70 to-transparent p-5 text-white">
                    <p className="font-semibold">{h.destination}</p>
                    <p className="text-sm text-zinc-200">{h.caption}</p>
                  </div>
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">{t.highlights.empty}</p>
          )}
        </div>
      </section>

      {/* Social */}
      <section id="social" className="mx-auto w-full max-w-6xl scroll-mt-20 px-6 py-24">
        <Reveal>
          <SectionHeading eyebrow={t.social.eyebrow} title={t.social.title} />
        </Reveal>
        {socialLinks.length > 0 ? (
          <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-5">
            {socialLinks.map((s, i) => (
              <Reveal
                as="div"
                key={s.platform}
                delay={i * 0.06}
              >
              <a
                href={s.url}
                target="_blank"
                rel="noopener noreferrer"
                className="block rounded-2xl bg-white p-6 text-center shadow-sm ring-1 ring-zinc-900/5 transition hover:shadow-lg"
              >
                <p className="font-semibold text-zinc-900">{s.platform}</p>
                {s.description && (
                  <p className="mt-2 text-sm text-zinc-500">{s.description}</p>
                )}
                {s.followerCount != null && (
                  <p className="mt-2 text-sm font-medium text-amber-600">
                    {s.followerCount.toLocaleString(locale)} {t.common.followers}
                  </p>
                )}
              </a>
              </Reveal>
            ))}
          </div>
        ) : (
          <p className="mt-8 text-center text-zinc-500">{t.social.empty}</p>
        )}
      </section>

      {/* Collaborations */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading
              eyebrow={t.collaborations.eyebrow}
              title={t.collaborations.title}
              description={t.collaborations.description}
            />
          </Reveal>
          {collaborations.length > 0 ? (
            <div className="mt-12 grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-3">
              {collaborations.map((c, i) => (
                <Reveal
                  as="div"
                  key={c.brandName}
                  delay={i * 0.08}
                  className="rounded-2xl bg-zinc-50 p-6 ring-1 ring-zinc-900/5"
                >
                  {c.logoUrl && (
                    <Image
                      src={c.logoUrl}
                      alt={c.logoAlt || c.brandName}
                      width={120}
                      height={60}
                      className="mb-4 h-10 w-auto object-contain"
                    />
                  )}
                  <p className="font-semibold text-zinc-900">{c.brandName}</p>
                  <p className="mt-2 text-sm text-zinc-600">{c.description}</p>
                  {c.testimonial && (
                    <p className="mt-3 text-sm italic text-zinc-500">
                      &ldquo;{c.testimonial}&rdquo;
                    </p>
                  )}
                  {c.campaignImages.length > 0 && (
                    <div className="mt-4 grid grid-cols-2 gap-2">
                      {c.campaignImages.map((image) => (
                        <div
                          key={image.url}
                          className="relative aspect-[4/3] overflow-hidden rounded-lg bg-zinc-100"
                        >
                          <Image
                            src={image.url}
                            alt={image.alt || c.brandName}
                            fill
                            className="object-cover"
                            sizes="(min-width: 1024px) 16vw, (min-width: 640px) 25vw, 50vw"
                          />
                        </div>
                      ))}
                    </div>
                  )}
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">{t.collaborations.empty}</p>
          )}
        </div>
      </section>

      {/* Featured Guides */}
      <section className="mx-auto w-full max-w-6xl px-6 py-24">
        <Reveal>
          <SectionHeading eyebrow={t.featuredGuides.eyebrow} title={t.featuredGuides.title} />
        </Reveal>
        {featuredGuides.length > 0 ? (
          <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featuredGuides.map((g, i) => (
              <Reveal as="div" key={g.slug} delay={i * 0.08}>
                <GuideCard guide={g} locale={locale} />
              </Reveal>
            ))}
          </div>
        ) : (
          <p className="mt-8 text-center text-zinc-500">{t.featuredGuides.empty}</p>
        )}
        <div className="mt-10 text-center">
          <Link
            href="/guides"
            className="text-sm font-semibold text-amber-600 hover:text-amber-500"
          >
            {t.common.viewAllGuides}
          </Link>
        </div>
      </section>

      {/* Latest News */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading eyebrow={t.latestNews.eyebrow} title={t.latestNews.title} />
          </Reveal>
          {latestNews.length > 0 ? (
            <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {latestNews.map((a, i) => (
                <Reveal as="div" key={a.slug} delay={i * 0.08}>
                  <NewsCard article={a} locale={locale} />
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">{t.latestNews.empty}</p>
          )}
          <div className="mt-10 text-center">
            <Link
              href="/news"
              className="text-sm font-semibold text-amber-600 hover:text-amber-500"
            >
              {t.common.viewAllNews}
            </Link>
          </div>
        </div>
      </section>

      {/* Contact */}
      <section
        id="contact"
        className="mx-auto w-full max-w-3xl scroll-mt-20 px-6 py-24 text-center"
      >
        <Reveal>
          <SectionHeading
            eyebrow={t.contact.eyebrow}
            title={contact?.headline || t.contact.fallbackTitle}
            description={contact?.body || t.contact.fallbackBody}
          />
          {contact?.email && (
            <a
              href={`mailto:${contact.email}`}
              className="mt-8 inline-block rounded-full bg-zinc-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-zinc-700"
            >
              {contact.email}
            </a>
          )}
        </Reveal>
      </section>
    </main>
  );
}
