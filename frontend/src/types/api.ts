export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

/** A public image reference with editor-authored (localized) alt text. */
export interface ImageMeta {
  url: string;
  alt: string | null;
}

/** Editable per-page SEO metadata resolved from the CMS by slug. */
export interface PageMeta {
  slug: string;
  title: string;
  metaTitle: string | null;
  metaDescription: string | null;
  ogImageUrl: string | null;
}

export interface NewsArticleSummary {
  slug: string;
  title: string;
  excerpt: string;
  coverImageUrl: string | null;
  coverImageAlt: string | null;
  publishedAt: string | null;
  readingTimeMinutes: number;
  category: string | null;
}

export interface NewsArticleDetail extends NewsArticleSummary {
  content: string;
  tags: string[];
  metaTitle: string | null;
  metaDescription: string | null;
  ogImageUrl: string | null;
}

export interface TravelGuideSummary {
  slug: string;
  title: string;
  country: string;
  city: string | null;
  continent: string;
  durationDays: number;
  difficulty: string | null;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
  coverImageUrl: string | null;
  coverImageAlt: string | null;
}

export interface TravelGuideDetail extends TravelGuideSummary {
  description: string;
  whatsIncluded: string | null;
  previewImages: ImageMeta[];
  lastUpdatedAt: string | null;
  metaTitle: string | null;
  metaDescription: string | null;
  ogImageUrl: string | null;
}

export type PurchaseStatus = "pending" | "completed" | "failed";

export interface PurchaseStatusResponse {
  status: PurchaseStatus;
  guideTitle: string;
  downloadUrl: string | null;
}

export interface SocialLink {
  platform: string;
  url: string;
  description: string | null;
  followerCount: number | null;
}

export interface Collaboration {
  brandName: string;
  description: string;
  testimonial: string | null;
  logoUrl: string | null;
  logoAlt: string | null;
  campaignImages: ImageMeta[];
}

export interface TravelHighlight {
  destination: string;
  caption: string;
  imageUrl: string | null;
  imageAlt: string | null;
}

export interface HomePage {
  hero: { headline: string; body: string; backgroundMediaUrl: string | null };
  about: { headline: string; body: string };
  contact: { headline: string; body: string; email: string | null };
  travelHighlights: TravelHighlight[];
  socialLinks: SocialLink[];
  collaborations: Collaboration[];
  featuredGuides: TravelGuideSummary[];
  latestNews: NewsArticleSummary[];
}
