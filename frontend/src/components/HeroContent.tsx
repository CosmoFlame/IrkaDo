"use client";

import Link from "next/link";
import { m } from "framer-motion";
import type { Variants } from "framer-motion";
import type { Locale } from "@/i18n/config";
import { getDictionary } from "@/i18n/dictionaries";

// Staggered intro for the hero copy. Runs once on mount (above the fold, so no
// scroll trigger). Reduced-motion is honoured globally via MotionConfig.
const container: Variants = {
  hidden: {},
  show: { transition: { staggerChildren: 0.12, delayChildren: 0.1 } },
};

const item: Variants = {
  hidden: { opacity: 0, y: 20 },
  show: { opacity: 1, y: 0, transition: { duration: 0.6, ease: [0.22, 1, 0.36, 1] } },
};

export function HeroContent({
  headline,
  body,
  locale,
}: {
  headline: string;
  body: string;
  locale: Locale;
}) {
  const t = getDictionary(locale);
  return (
    <m.div
      className="relative z-10 mx-auto w-full max-w-6xl px-6 pb-20"
      variants={container}
      initial="hidden"
      animate="show"
    >
      <m.p
        variants={item}
        className="text-sm font-medium uppercase tracking-[0.3em] text-amber-400"
      >
        {t.hero.eyebrow}
      </m.p>
      <m.h1
        variants={item}
        className="mt-4 max-w-2xl text-5xl font-semibold tracking-tight sm:text-6xl"
      >
        {headline}
      </m.h1>
      <m.p variants={item} className="mt-6 max-w-xl text-lg text-zinc-200">
        {body}
      </m.p>
      <m.div variants={item} className="mt-8 flex flex-wrap gap-4">
        <Link
          href="/guides"
          className="rounded-full bg-amber-500 px-6 py-3 text-sm font-semibold text-zinc-900 transition hover:bg-amber-400"
        >
          {t.hero.exploreGuides}
        </Link>
        <Link
          href="#social"
          className="rounded-full border border-white/30 px-6 py-3 text-sm font-semibold transition hover:bg-white/10"
        >
          {t.hero.followJourney}
        </Link>
        <Link
          href="#contact"
          className="rounded-full border border-white/30 px-6 py-3 text-sm font-semibold transition hover:bg-white/10"
        >
          {t.hero.contactMe}
        </Link>
      </m.div>
    </m.div>
  );
}
