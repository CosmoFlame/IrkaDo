export interface NewsArticleSummary {
  slug: string;
  title: string;
  excerpt: string;
  coverImageUrl: string | null;
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
}

export interface TravelGuideDetail extends TravelGuideSummary {
  description: string;
  whatsIncluded: string | null;
  previewImageUrls: string[];
  lastUpdatedAt: string | null;
  metaTitle: string | null;
  metaDescription: string | null;
  ogImageUrl: string | null;
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
  campaignImageUrls: string[];
}

export interface TravelHighlight {
  destination: string;
  caption: string;
  imageUrl: string | null;
}

export interface HomePage {
  hero: { headline: string; body: string; backgroundMediaUrl: string | null };
  about: { headline: string; body: string };
  contact: { headline: string; body: string };
  travelHighlights: TravelHighlight[];
  socialLinks: SocialLink[];
  collaborations: Collaboration[];
  featuredGuides: TravelGuideSummary[];
  latestNews: NewsArticleSummary[];
}
