import type {
  Collaboration,
  HomePage,
  NewsArticleDetail,
  NewsArticleSummary,
  SocialLink,
  TravelGuideDetail,
  TravelGuideSummary,
} from "@/types/api";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

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

export const getHomePage = () => apiFetch<HomePage>("/home");

export const getNewsArticles = (params?: { category?: string; page?: number }) => {
  const query = new URLSearchParams();
  if (params?.category) query.set("category", params.category);
  if (params?.page) query.set("page", String(params.page));
  const qs = query.toString();
  return apiFetch<NewsArticleSummary[]>(`/news${qs ? `?${qs}` : ""}`);
};

export const getNewsArticleBySlug = (slug: string) =>
  apiFetch<NewsArticleDetail>(`/news/${slug}`, 60);

export const getTravelGuides = (params?: {
  country?: string;
  continent?: string;
  type?: "free" | "premium";
}) => {
  const query = new URLSearchParams();
  if (params?.country) query.set("country", params.country);
  if (params?.continent) query.set("continent", params.continent);
  if (params?.type) query.set("type", params.type);
  const qs = query.toString();
  return apiFetch<TravelGuideSummary[]>(`/guides${qs ? `?${qs}` : ""}`);
};

export const getTravelGuideBySlug = (slug: string) =>
  apiFetch<TravelGuideDetail>(`/guides/${slug}`, 60);

export const getSocialLinks = () => apiFetch<SocialLink[]>("/social-links");

export const getCollaborations = () => apiFetch<Collaboration[]>("/collaborations");
