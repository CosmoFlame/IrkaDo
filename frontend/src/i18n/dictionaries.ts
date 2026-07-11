import type { Locale } from "./config";

// Static UI copy (labels, buttons, empty states). Business content — article titles,
// guide descriptions, home-section text — is localized by the API, not here.
// `uk` is the default; `en` mirrors its shape exactly (enforced by the `Dictionary` type).

const uk = {
  nav: {
    home: "Головна",
    guides: "Гайди",
    news: "Новини",
    contact: "Контакти",
    openMenu: "Відкрити меню",
    closeMenu: "Закрити меню",
    language: "Мова",
  },
  common: {
    noImage: "Ще без зображення",
    free: "Безкоштовно",
    viewAllGuides: "Усі гайди для подорожей →",
    viewAllNews: "Усі новини →",
    followers: "підписників",
    minRead: "хв читання",
    day: "день",
    days: "днів",
    links: "Посилання",
  },
  hero: {
    eyebrow: "Тревел-креаторка",
    exploreGuides: "Переглянути гайди",
    followJourney: "Стежити за подорожжю",
    contactMe: "Написати мені",
    fallbackHeadline: "Мандруй далеко. Живи вільно. Розкажи історію.",
    fallbackBody:
      "Ірина Долженко (Irka_do) — гайди для подорожей, історії та пригоди з усього світу.",
  },
  about: {
    eyebrow: "Про Ірку",
    fallbackTitle: "Мандрівниця в душі, оповідачка за покликанням",
    fallbackBody:
      "Контент незабаром — цей блок працює від Home API і покаже біографію Ірини, філософію подорожей і досвід, щойно з'явиться реальний контент.",
  },
  social: {
    eyebrow: "Будьмо на зв'язку",
    title: "Стежте за подорожжю",
    empty: "Посилання на соцмережі з'являться тут після налаштування.",
  },
  collaborations: {
    eyebrow: "Партнерства",
    title: "Співпраця з брендами",
    description:
      "Відкрита до майбутніх партнерств із готелями, авіакомпаніями, туристичними радами та lifestyle-брендами.",
    empty: "Кейси співпраці з'являться тут незабаром.",
  },
  featuredGuides: {
    eyebrow: "Плануй подорож",
    title: "Обрані гайди для подорожей",
    empty: "Гайдів ще не опубліковано — зазирни згодом.",
  },
  latestNews: {
    eyebrow: "Журнал",
    title: "Новини та думки",
    empty: "Статей ще не опубліковано — зазирни згодом.",
  },
  contact: {
    eyebrow: "Співпраця",
    fallbackTitle: "Співпрацюймо",
    fallbackBody:
      "Щодо співпраці, реклами та партнерств — звертайтеся нижче.",
  },
  guidesPage: {
    eyebrow: "Плануй подорож",
    title: "Гайди для подорожей",
    filterCountry: "Країна",
    filterByCountry: "Фільтрувати за країною",
    filterByPrice: "Фільтрувати за ціною",
    all: "Усі",
    free: "Безкоштовні",
    premium: "Преміум",
    filter: "Фільтрувати",
    empty: "Немає гайдів, що відповідають фільтрам.",
    metaTitle: "Гайди для подорожей",
    metaDescription: "Безкоштовні та преміум гайди для подорожей від Irka_do.",
  },
  guideDetail: {
    whatsIncluded: "Що входить",
    updated: "Оновлено",
    previewAlt: "Прев'ю гайда",
  },
  newsPage: {
    eyebrow: "Журнал",
    title: "Новини подорожей",
    empty: "Статей ще не опубліковано — зазирни згодом.",
    previous: "Назад",
    next: "Далі",
    pageOf: (page: number, total: number) => `Сторінка ${page} з ${total}`,
    metaTitle: "Новини та історії подорожей",
    metaDescription: "Останні новини, історії та оновлення про подорожі від Irka_do.",
  },
  purchase: {
    readyTitle: "Дякуємо за покупку!",
    readyBody: "Твій гайд готовий. Ми також надіслали копію цього посилання на пошту.",
    download: "Завантажити гайд",
    failedTitle: "Оплата не пройшла",
    failedBody: "Кошти з картки не списано. Можеш спробувати ще раз нижче.",
    tryAgain: "Спробувати ще раз",
    pendingTitle: "Оплата обробляється",
    pendingBody:
      "Дякуємо за покупку! Посилання для завантаження вже прямує на твою пошту — воно має надійти за кілька хвилин.",
    confirmingTitle: "Підтверджуємо оплату…",
    confirmingBody: "Це займе лише мить. Будь ласка, не закривай сторінку.",
    cancelled: "Оплату скасовано — кошти не списано. Можеш продовжити з того місця, де зупинився, нижче.",
    redirecting: "Перенаправляємо…",
    buy: "Купити цей гайд",
    somethingWrong: "Щось пішло не так. Спробуй ще раз.",
    preparing: "Готуємо завантаження…",
    downloadFree: "Завантажити безкоштовний гайд",
    rateLimited: "Забагато спроб — спробуй ще раз за хвилину.",
  },
  layout: {
    skipToContent: "Перейти до вмісту",
    metaDescription:
      "Гайди для подорожей, історії та пригоди від Ірини Долженко (Irka_do).",
  },
};

// English mirrors the `uk` shape. `satisfies Dictionary` guarantees no key drifts.
const en = {
  nav: {
    home: "Home",
    guides: "Guides",
    news: "News",
    contact: "Contact",
    openMenu: "Open menu",
    closeMenu: "Close menu",
    language: "Language",
  },
  common: {
    noImage: "No image yet",
    free: "Free",
    viewAllGuides: "View all travel guides →",
    viewAllNews: "View all news →",
    followers: "followers",
    minRead: "min read",
    day: "day",
    days: "days",
    links: "Links",
  },
  hero: {
    eyebrow: "Travel Creator",
    exploreGuides: "Explore Travel Guides",
    followJourney: "Follow My Journey",
    contactMe: "Contact Me",
    fallbackHeadline: "Wander far. Live free. Tell the story.",
    fallbackBody:
      "Iryna Dolzhenko (Irka_do) — travel guides, stories, and adventures from around the world.",
  },
  about: {
    eyebrow: "About Irka",
    fallbackTitle: "A traveler at heart, a storyteller by craft",
    fallbackBody:
      "Content coming soon — this section is powered by the Home API and will show Iryna's bio, travel philosophy, and experience once real content is added.",
  },
  social: {
    eyebrow: "Stay Connected",
    title: "Follow the Journey",
    empty: "Social links will appear here once configured.",
  },
  collaborations: {
    eyebrow: "Partnerships",
    title: "Brand Collaborations",
    description:
      "Open for future partnerships with hotels, airlines, tourism boards, and lifestyle brands.",
    empty: "Collaboration case studies will appear here soon.",
  },
  featuredGuides: {
    eyebrow: "Plan Your Trip",
    title: "Featured Travel Guides",
    empty: "No guides published yet — check back soon.",
  },
  latestNews: {
    eyebrow: "Journal",
    title: "News & Thoughts",
    empty: "No articles published yet — check back soon.",
  },
  contact: {
    eyebrow: "Work Together",
    fallbackTitle: "Let's Collaborate",
    fallbackBody:
      "For collaborations, advertising, and partnership inquiries, reach out below.",
  },
  guidesPage: {
    eyebrow: "Plan Your Trip",
    title: "Travel Guides",
    filterCountry: "Country",
    filterByCountry: "Filter by country",
    filterByPrice: "Filter by price",
    all: "All",
    free: "Free",
    premium: "Premium",
    filter: "Filter",
    empty: "No guides match your filters yet.",
    metaTitle: "Travel Guides",
    metaDescription: "Free and premium travel guides curated by Irka_do.",
  },
  guideDetail: {
    whatsIncluded: "What's Included",
    updated: "Updated",
    previewAlt: "Guide preview",
  },
  newsPage: {
    eyebrow: "Journal",
    title: "Travel News",
    empty: "No articles published yet — check back soon.",
    previous: "Previous",
    next: "Next",
    pageOf: (page: number, total: number) => `Page ${page} of ${total}`,
    metaTitle: "Travel News & Stories",
    metaDescription: "The latest travel news, stories, and updates from Irka_do.",
  },
  purchase: {
    readyTitle: "Thank you for your purchase!",
    readyBody: "Your guide is ready. We've also emailed you a copy of this link.",
    download: "Download your guide",
    failedTitle: "Payment didn't go through",
    failedBody: "Your card was not charged. You can try again below.",
    tryAgain: "Try Again",
    pendingTitle: "Your payment is being processed",
    pendingBody:
      "Thanks for your purchase! Your download link is on its way to your email — it should arrive within a few minutes.",
    confirmingTitle: "Confirming your payment…",
    confirmingBody: "This only takes a moment. Please don't close this page.",
    cancelled:
      "Checkout cancelled — no charge was made. You can pick up where you left off below.",
    redirecting: "Redirecting…",
    buy: "Buy This Guide",
    somethingWrong: "Something went wrong. Please try again.",
    preparing: "Preparing download…",
    downloadFree: "Download Free Guide",
    rateLimited: "Too many attempts — please try again in a minute.",
  },
  layout: {
    skipToContent: "Skip to content",
    metaDescription:
      "Travel guides, stories, and adventures from Iryna Dolzhenko (Irka_do).",
  },
};

export type Dictionary = typeof uk;

const dictionaries: Record<Locale, Dictionary> = {
  uk,
  // `en` is validated against the `uk` shape at compile time.
  en: en satisfies Dictionary,
};

export function getDictionary(locale: Locale): Dictionary {
  return dictionaries[locale];
}
