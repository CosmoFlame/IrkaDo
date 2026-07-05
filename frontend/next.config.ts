import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      { protocol: "https", hostname: "**" },
      { protocol: "http", hostname: "localhost", port: "5289" },
    ],
    // Next's built-in image optimizer refuses to proxy hosts resolving to private/loopback
    // IPs (SSRF protection), which localhost always does. No server-side resize/optimize;
    // images render as-is. In production these would come from real cloud storage (S3/Azure
    // Blob/R2) over public HTTPS, where optimization would work normally.
    unoptimized: true,
  },
};

export default nextConfig;
