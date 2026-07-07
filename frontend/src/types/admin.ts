// Mirrors the backend admin DTOs (IrkaDo.Application/Features/Admin).
// Enums are serialized as strings (JsonStringEnumConverter on the API).

export interface AdminLoginResponse {
  token: string;
  expiresAt: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

// --- News ---
export interface AdminNewsListItem {
  id: string;
  slug: string;
  title: string;
  isPublished: boolean;
  publishedAt: string | null;
  category: string | null;
  coverImageUrl: string | null;
  updatedAt: string | null;
}

export interface AdminNewsDetail {
  id: string;
  slug: string;
  title: string;
  titleEn: string | null;
  excerpt: string;
  excerptEn: string | null;
  content: string;
  contentEn: string | null;
  readingTimeMinutes: number;
  isPublished: boolean;
  publishedAt: string | null;
  coverImageId: string;
  coverImageUrl: string | null;
  categoryId: string;
  tagIds: string[];
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}

export interface AdminNewsUpsert {
  title: string;
  titleEn: string | null;
  slug: string;
  excerpt: string;
  excerptEn: string | null;
  content: string;
  contentEn: string | null;
  readingTimeMinutes: number;
  isPublished: boolean;
  coverImageId: string;
  categoryId: string;
  tagIds: string[];
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}

// --- Guides ---
export type GuideDifficulty = "Easy" | "Moderate" | "Challenging";

export interface AdminGuideFile {
  id: string;
  fileName: string;
  sizeBytes: number;
}

export interface AdminGuideListItem {
  id: string;
  slug: string;
  title: string;
  country: string;
  continent: string;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
  isPublished: boolean;
  isFeatured: boolean;
  coverImageUrl: string | null;
  fileCount: number;
  updatedAt: string | null;
}

export interface AdminGuideDetail {
  id: string;
  slug: string;
  title: string;
  titleEn: string | null;
  country: string;
  countryEn: string | null;
  city: string | null;
  cityEn: string | null;
  continent: string;
  continentEn: string | null;
  description: string;
  descriptionEn: string | null;
  whatsIncluded: string | null;
  whatsIncludedEn: string | null;
  durationDays: number;
  difficulty: GuideDifficulty | null;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
  isPublished: boolean;
  isFeatured: boolean;
  lastUpdatedAt: string | null;
  coverImageId: string;
  coverImageUrl: string | null;
  previewImageIds: string[];
  files: AdminGuideFile[];
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}

export interface AdminGuideUpsert {
  title: string;
  titleEn: string | null;
  slug: string;
  country: string;
  countryEn: string | null;
  city: string | null;
  cityEn: string | null;
  continent: string;
  continentEn: string | null;
  description: string;
  descriptionEn: string | null;
  whatsIncluded: string | null;
  whatsIncludedEn: string | null;
  durationDays: number;
  difficulty: GuideDifficulty | null;
  isPremium: boolean;
  priceAmount: number | null;
  priceCurrency: string;
  isPublished: boolean;
  isFeatured: boolean;
  coverImageId: string;
  previewImageIds: string[];
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}

// --- Collaborations ---
export interface AdminCollaboration {
  id: string;
  brandName: string;
  description: string;
  descriptionEn: string | null;
  testimonial: string | null;
  testimonialEn: string | null;
  displayOrder: number;
  isPublished: boolean;
  logoId: string;
  logoUrl: string | null;
  campaignImageIds: string[];
  campaignImageUrls: string[];
}

export interface AdminCollaborationUpsert {
  brandName: string;
  description: string;
  descriptionEn: string | null;
  testimonial: string | null;
  testimonialEn: string | null;
  displayOrder: number;
  isPublished: boolean;
  logoId: string;
  campaignImageIds: string[];
}

// --- Social links ---
export type SocialPlatform = "Instagram" | "TikTok" | "YouTube" | "Telegram" | "Threads";

export interface AdminSocialLink {
  id: string;
  platform: SocialPlatform;
  url: string;
  description: string | null;
  descriptionEn: string | null;
  followerCount: number | null;
  displayOrder: number;
}

export interface AdminSocialLinkUpsert {
  platform: SocialPlatform;
  url: string;
  description: string | null;
  descriptionEn: string | null;
  followerCount: number | null;
  displayOrder: number;
}

// --- Highlights ---
export interface AdminHighlight {
  id: string;
  destination: string;
  destinationEn: string | null;
  caption: string;
  captionEn: string | null;
  displayOrder: number;
  isPublished: boolean;
  imageId: string;
  imageUrl: string | null;
}

export interface AdminHighlightUpsert {
  destination: string;
  destinationEn: string | null;
  caption: string;
  captionEn: string | null;
  displayOrder: number;
  isPublished: boolean;
  imageId: string;
}

// --- Home sections ---
export type HomeSectionType = "Hero" | "About" | "Contact";

export interface AdminHomeSection {
  id: string;
  type: HomeSectionType;
  headline: string;
  headlineEn: string | null;
  body: string;
  bodyEn: string | null;
  contentJson: string;
  contentJsonEn: string | null;
  backgroundMediaId: string | null;
  backgroundMediaUrl: string | null;
}

export interface AdminHomeSectionUpdate {
  headline: string;
  headlineEn: string | null;
  body: string;
  bodyEn: string | null;
  contentJson: string;
  contentJsonEn: string | null;
  backgroundMediaId: string | null;
}

// --- Taxonomy ---
export interface AdminCategory {
  id: string;
  name: string;
  nameEn: string | null;
  slug: string;
}
export interface AdminCategoryUpsert {
  name: string;
  nameEn: string | null;
  slug: string;
}
export interface AdminTag {
  id: string;
  name: string;
  nameEn: string | null;
  slug: string;
}
export interface AdminTagUpsert {
  name: string;
  nameEn: string | null;
  slug: string;
}
export interface AdminPage {
  id: string;
  slug: string;
  title: string;
  titleEn: string | null;
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}
export interface AdminPageUpsert {
  slug: string;
  title: string;
  titleEn: string | null;
  metaTitle: string | null;
  metaTitleEn: string | null;
  metaDescription: string | null;
  metaDescriptionEn: string | null;
  ogImageUrl: string | null;
}

// --- Media ---
export type MediaAssetType = "Image" | "Video" | "Document";
export interface AdminMedia {
  id: string;
  url: string;
  type: MediaAssetType;
  altText: string | null;
  altTextEn: string | null;
  width: number | null;
  height: number | null;
  createdAt: string;
}

// --- Reporting ---
export type PurchaseStatus = "Pending" | "Completed" | "Failed";
export interface AdminPurchase {
  id: string;
  guideTitle: string | null;
  customerEmail: string;
  amountPaid: number;
  currency: string;
  status: PurchaseStatus;
  paymentProvider: string;
  providerSessionId: string;
  deliveredAt: string | null;
  createdAt: string;
}
export interface AdminDownloadLog {
  id: string;
  guideTitle: string | null;
  email: string | null;
  ipAddress: string;
  createdAt: string;
}
export interface AdminDashboard {
  newsTotal: number;
  newsPublished: number;
  guidesTotal: number;
  guidesPublished: number;
  premiumGuides: number;
  collaborations: number;
  socialLinks: number;
  highlights: number;
  mediaAssets: number;
  purchasesCompleted: number;
  revenue: number;
  downloadsTotal: number;
}
