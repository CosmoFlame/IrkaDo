import type { Locale } from "@/i18n/config";
import type {
  Collaboration,
  HomePage,
  NewsArticleDetail,
  NewsArticleSummary,
  PageMeta,
  PagedResult,
  SocialLink,
  TravelGuideDetail,
  TravelGuideSummary,
} from "@/types/api";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

// Appends the content language to the query string so the API returns localized fields and each
// language gets a distinct cache key. Omitted for language-neutral calls (e.g. slug enumeration).
function withLang(query: URLSearchParams, lang?: Locale): string {
  if (lang) query.set("lang", lang);
  const qs = query.toString();
  return qs ? `?${qs}` : "";
}

async function apiFetch<T>(path: string, revalidateSeconds = 60): Promise<T | null> {
  try {
    const res = await fetch(`${API_BASE_URL}/api/v1${path}`, {
      next: { revalidate: revalidateSeconds },
    });
    if (!res.ok) return null;
    return (await res.json()) as T;
  } catch {
    return null;
  }
}

export const getHomePage = (lang?: Locale) =>
  apiFetch<HomePage>(`/home${withLang(new URLSearchParams(), lang)}`);

// Editable per-page SEO metadata (slug e.g. "home", "news", "guides"). Returns null if the page
// has no CMS entry yet, so callers fall back to their static dictionary copy.
export const getPageMeta = (slug: string, lang?: Locale) =>
  apiFetch<PageMeta>(`/pages/${slug}${withLang(new URLSearchParams(), lang)}`);

export const NEWS_PAGE_SIZE = 9;

export const getNewsArticles = (params?: {
  category?: string;
  page?: number;
  pageSize?: number;
  lang?: Locale;
}) => {
  const query = new URLSearchParams();
  if (params?.category) query.set("category", params.category);
  if (params?.page) query.set("page", String(params.page));
  query.set("pageSize", String(params?.pageSize ?? NEWS_PAGE_SIZE));
  return apiFetch<PagedResult<NewsArticleSummary>>(`/news${withLang(query, params?.lang)}`);
};

export const getAllNewsSlugs = async () => {
  const result = await getNewsArticles({ pageSize: 500 });
  return result?.items.map((article) => article.slug) ?? [];
};

export const getAllGuideSlugs = async () => {
  const guides = await getTravelGuides();
  return guides?.map((guide) => guide.slug) ?? [];
};

export const getNewsArticleBySlug = (slug: string, lang?: Locale) =>
  apiFetch<NewsArticleDetail>(`/news/${slug}${withLang(new URLSearchParams(), lang)}`, 60);

export const getTravelGuides = (params?: {
  country?: string;
  continent?: string;
  type?: "free" | "premium";
  lang?: Locale;
}) => {
  const query = new URLSearchParams();
  if (params?.country) query.set("country", params.country);
  if (params?.continent) query.set("continent", params.continent);
  if (params?.type) query.set("type", params.type);
  return apiFetch<TravelGuideSummary[]>(`/guides${withLang(query, params?.lang)}`);
};

export const getTravelGuideBySlug = (slug: string, lang?: Locale) =>
  apiFetch<TravelGuideDetail>(`/guides/${slug}${withLang(new URLSearchParams(), lang)}`, 60);

export const getSocialLinks = (lang?: Locale) =>
  apiFetch<SocialLink[]>(`/social-links${withLang(new URLSearchParams(), lang)}`);

export const getCollaborations = (lang?: Locale) =>
  apiFetch<Collaboration[]>(`/collaborations${withLang(new URLSearchParams(), lang)}`);
