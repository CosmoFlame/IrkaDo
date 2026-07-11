import type { Metadata } from "next";
import Image from "next/image";
import Link from "next/link";
import { getHomePage } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { GuideCard } from "@/components/GuideCard";
import { NewsCard } from "@/components/NewsCard";
import { ContentLinks } from "@/components/ContentLinks";
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
      <section className="bg-white">
        <div className="mx-auto w-full max-w-4xl px-6 py-16 sm:py-24">
          <Reveal>
            <SectionHeading
              eyebrow={t.about.eyebrow}
              title={about?.headline || t.about.fallbackTitle}
              description={about?.body || t.about.fallbackBody}
            />
          </Reveal>
        </div>
      </section>

      {/* Featured Guides */}
      <section className="bg-zinc-50">
        <div className="mx-auto w-full max-w-6xl px-6 py-16 sm:py-24">
          <Reveal>
            <SectionHeading eyebrow={t.featuredGuides.eyebrow} title={t.featuredGuides.title} />
          </Reveal>
          {featuredGuides.length > 0 ? (
            <div className="mt-8 grid grid-cols-1 gap-4 sm:mt-12 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
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
        </div>
      </section>

      {/* Collaborations — presented like news cards (cover, name, description, testimonial, links) */}
      <section className="bg-white py-16 sm:py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading
              eyebrow={t.collaborations.eyebrow}
              title={t.collaborations.title}
              description={t.collaborations.description}
            />
          </Reveal>
          {collaborations.length > 0 ? (
            <div className="mt-8 grid grid-cols-1 gap-4 sm:mt-12 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
              {collaborations.map((c, i) => (
                <Reveal
                  as="div"
                  key={c.brandName}
                  delay={i * 0.08}
                  className="overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-zinc-900/5"
                >
                  <div className="relative aspect-[16/10] w-full overflow-hidden bg-zinc-100">
                    {c.coverImageUrl && (
                      <Image
                        src={c.coverImageUrl}
                        alt={c.coverImageAlt || c.brandName}
                        fill
                        className="object-cover"
                        sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
                      />
                    )}
                  </div>
                  <div className="p-4 sm:p-5">
                    <h3 className="text-base font-semibold text-zinc-900 sm:text-lg">{c.brandName}</h3>
                    <p className="mt-2 text-sm text-zinc-600">{c.description}</p>
                    {c.testimonial && (
                      <p className="mt-3 text-sm italic text-zinc-500">&ldquo;{c.testimonial}&rdquo;</p>
                    )}
                    <ContentLinks links={c.links} compact className="mt-3 flex flex-wrap gap-x-4 gap-y-1" />
                  </div>
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">{t.collaborations.empty}</p>
          )}
        </div>
      </section>

      {/* Latest News */}
      <section className="bg-zinc-50">
        <div className="mx-auto w-full max-w-6xl px-6 py-16 sm:py-24">
          <Reveal>
            <SectionHeading eyebrow={t.latestNews.eyebrow} title={t.latestNews.title} />
          </Reveal>
          {latestNews.length > 0 ? (
            <div className="mt-8 grid grid-cols-1 gap-4 sm:mt-12 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
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

      {/* Social */}
      <section id="social" className="bg-white py-16 sm:py-24">
        <div className="mx-auto w-full max-w-6xl scroll-mt-20 px-6">
          <Reveal>
            <SectionHeading eyebrow={t.social.eyebrow} title={t.social.title} />
          </Reveal>
          {socialLinks.length > 0 ? (
            <div className="mt-8 grid grid-cols-1 gap-4 sm:mt-12 sm:grid-cols-2 sm:gap-6 lg:grid-cols-5">
              {socialLinks.map((s, i) => (
                <Reveal as="div" key={s.platform} delay={i * 0.06}>
                  <a
                    href={s.url}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="block rounded-2xl bg-white p-5 text-center shadow-sm ring-1 ring-zinc-900/5 transition hover:shadow-lg sm:p-6"
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
        </div>
      </section>

      {/* Contact */}
      <section id="contact" className="scroll-mt-20 bg-zinc-50">
        <div className="mx-auto w-full max-w-3xl px-6 py-16 text-center sm:py-24">
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
        </div>
      </section>
    </main>
  );
}
