import type { MetadataRoute } from "next";
import { getAllGuideSlugs, getAllNewsSlugs } from "@/lib/api";

const SITE_URL = process.env.NEXT_PUBLIC_SITE_URL ?? "http://localhost:3000";

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const [newsSlugs, guideSlugs] = await Promise.all([getAllNewsSlugs(), getAllGuideSlugs()]);

  const staticRoutes: MetadataRoute.Sitemap = ["", "/news", "/guides"].map((path) => ({
    url: `${SITE_URL}${path}`,
  }));

  const newsRoutes: MetadataRoute.Sitemap = newsSlugs.map((slug) => ({
    url: `${SITE_URL}/news/${slug}`,
  }));

  const guideRoutes: MetadataRoute.Sitemap = guideSlugs.map((slug) => ({
    url: `${SITE_URL}/guides/${slug}`,
  }));

  return [...staticRoutes, ...newsRoutes, ...guideRoutes];
}
