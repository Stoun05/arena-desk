import type { NextConfig } from "next";

const isGitHubPages = process.env.GITHUB_PAGES === "true";

const nextConfig: NextConfig = {
  reactStrictMode: true,
  ...(isGitHubPages ? {
    output: "export",
    basePath: "/arena-desk",
    assetPrefix: "/arena-desk",
    trailingSlash: true,
    images: { unoptimized: true },
  } : {}),
};

export default nextConfig;
