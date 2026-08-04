import type { NextConfig } from "next";

const isGitHubPages = process.env.GITHUB_PAGES === "true";

const nextConfig: NextConfig = {
  reactStrictMode: true,
  output: isGitHubPages ? "export" : undefined,
  basePath: isGitHubPages ? "/arena-desk" : undefined,
  assetPrefix: isGitHubPages ? "/arena-desk/" : undefined,
  trailingSlash: isGitHubPages,
  images: { unoptimized: isGitHubPages },
};

export default nextConfig;
