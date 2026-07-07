import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { SiteNav } from "@/components/SiteNav";
import { MotionProvider } from "@/components/motion/MotionProvider";
import { getDictionary } from "@/i18n/dictionaries";
import { getLocale } from "@/i18n/server";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export async function generateMetadata(): Promise<Metadata> {
  const t = getDictionary(await getLocale());
  return {
    title: {
      default: "Irka_do — Travel Creator",
      template: "%s | Irka_do",
    },
    description: t.layout.metaDescription,
  };
}

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const locale = await getLocale();
  const t = getDictionary(locale);

  return (
    <html
      lang={locale}
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="flex min-h-full flex-col bg-zinc-50 text-zinc-900">
        <a
          href="#main-content"
          className="sr-only focus-visible:not-sr-only focus-visible:fixed focus-visible:left-4 focus-visible:top-4 focus-visible:z-[100] focus-visible:rounded-full focus-visible:bg-zinc-900 focus-visible:px-5 focus-visible:py-2 focus-visible:text-sm focus-visible:font-semibold focus-visible:text-white"
        >
          {t.layout.skipToContent}
        </a>
        <MotionProvider>
          <SiteNav locale={locale} />
          <div id="main-content" tabIndex={-1} className="flex flex-1 flex-col outline-none">
            {children}
          </div>
        </MotionProvider>
      </body>
    </html>
  );
}
