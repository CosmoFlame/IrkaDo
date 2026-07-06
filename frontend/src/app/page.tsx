import Image from "next/image";
import Link from "next/link";
import { getHomePage } from "@/lib/api";
import { SectionHeading } from "@/components/SectionHeading";
import { GuideCard } from "@/components/GuideCard";
import { NewsCard } from "@/components/NewsCard";
import { HeroContent } from "@/components/HeroContent";
import { Reveal } from "@/components/motion/Reveal";

export default async function Home() {
  const home = await getHomePage();

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
          headline={hero?.headline || "Wander far. Live free. Tell the story."}
          body={
            hero?.body ||
            "Iryna Dolzhenko (Irka_do) — travel guides, stories, and adventures from around the world."
          }
        />
      </section>

      {/* About */}
      <section className="mx-auto w-full max-w-4xl px-6 py-24">
        <Reveal>
          <SectionHeading
            eyebrow="About Irka"
            title={about?.headline || "A traveler at heart, a storyteller by craft"}
            description={
              about?.body ||
              "Content coming soon — this section is powered by the Home API and will show Iryna's bio, travel philosophy, and experience once real content is added."
            }
          />
        </Reveal>
      </section>

      {/* Travel Highlights */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading eyebrow="Adventures" title="Travel Highlights" />
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
                      alt={h.destination}
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
            <p className="mt-8 text-center text-zinc-500">
              Highlights will appear here once trips are added.
            </p>
          )}
        </div>
      </section>

      {/* Social */}
      <section id="social" className="mx-auto w-full max-w-6xl scroll-mt-20 px-6 py-24">
        <Reveal>
          <SectionHeading eyebrow="Stay Connected" title="Follow the Journey" />
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
                    {s.followerCount.toLocaleString()} followers
                  </p>
                )}
              </a>
              </Reveal>
            ))}
          </div>
        ) : (
          <p className="mt-8 text-center text-zinc-500">
            Social links will appear here once configured.
          </p>
        )}
      </section>

      {/* Collaborations */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading
              eyebrow="Partnerships"
              title="Brand Collaborations"
              description="Open for future partnerships with hotels, airlines, tourism boards, and lifestyle brands."
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
                      alt={c.brandName}
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
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">
              Collaboration case studies will appear here soon.
            </p>
          )}
        </div>
      </section>

      {/* Featured Guides */}
      <section className="mx-auto w-full max-w-6xl px-6 py-24">
        <Reveal>
          <SectionHeading eyebrow="Plan Your Trip" title="Featured Travel Guides" />
        </Reveal>
        {featuredGuides.length > 0 ? (
          <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featuredGuides.map((g, i) => (
              <Reveal as="div" key={g.slug} delay={i * 0.08}>
                <GuideCard guide={g} />
              </Reveal>
            ))}
          </div>
        ) : (
          <p className="mt-8 text-center text-zinc-500">
            No guides published yet — check back soon.
          </p>
        )}
        <div className="mt-10 text-center">
          <Link
            href="/guides"
            className="text-sm font-semibold text-amber-600 hover:text-amber-500"
          >
            View all travel guides →
          </Link>
        </div>
      </section>

      {/* Latest News */}
      <section className="bg-white py-24">
        <div className="mx-auto max-w-6xl px-6">
          <Reveal>
            <SectionHeading eyebrow="Journal" title="Latest News" />
          </Reveal>
          {latestNews.length > 0 ? (
            <div className="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {latestNews.map((a, i) => (
                <Reveal as="div" key={a.slug} delay={i * 0.08}>
                  <NewsCard article={a} />
                </Reveal>
              ))}
            </div>
          ) : (
            <p className="mt-8 text-center text-zinc-500">
              No articles published yet — check back soon.
            </p>
          )}
          <div className="mt-10 text-center">
            <Link
              href="/news"
              className="text-sm font-semibold text-amber-600 hover:text-amber-500"
            >
              View all news →
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
            eyebrow="Work Together"
            title={contact?.headline || "Let's Collaborate"}
            description={
              contact?.body ||
              "For collaborations, advertising, and partnership inquiries, reach out below."
            }
          />
          <a
            href="mailto:hello@irkado.com"
            className="mt-8 inline-block rounded-full bg-zinc-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-zinc-700"
          >
            hello@irkado.com
          </a>
        </Reveal>
      </section>
    </main>
  );
}
