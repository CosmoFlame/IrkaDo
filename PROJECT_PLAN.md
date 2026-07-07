# IrkaDo — Personal Travel Creator Website

**Project:** Personal website for travel creator Iryna Dolzhenko (Irka_do)
**Status:** Planning
**Last updated:** 2026-07-05

---

## 1. Project Overview

A modern, premium, visually-driven personal website acting as the central hub for
Iryna Dolzhenko's (Irka_do) content, social media presence, travel guides, and
brand collaborations. The site should feel like a modern travel brand, not a
typical personal blog — emotionally evocative, minimal, fast, and easy to
navigate on any device.

The system must be **fully data-driven** (no hardcoded content) so a future
admin panel can manage everything, and the architecture must allow new pages
and features to be added without major redesign.

### Design Pillars
- Travel, freedom, adventure, authenticity, trust, premium creator brand
- Large photography, elegant typography, generous whitespace
- Smooth, subtle animations that never hurt usability
- Minimal, uncluttered layouts

---

## 2. Information Architecture

### 2.1 Home Page
| Section | Purpose |
|---|---|
| Hero | Large background image/video, intro line, slogan, CTA buttons (Explore Guides / Follow My Journey / Contact) |
| About | Bio, countries visited, philosophy, content style, brand experience |
| Travel Highlights | Curated cards of past trips/destinations/moments |
| Social Media | Instagram, TikTok, YouTube, Telegram, Threads — icon, description, follower count (future), visit button |
| Brand Collaborations | Logos, campaign images, description, optional testimonial, open-for-partnership CTA |
| Featured Travel Guides | Cover, country, description, free/premium badge, CTA |
| Latest News Preview | Recent posts + link to full News page |
| Contact | Business email, social links, (future) contact form |

### 2.2 News Page
- Card grid: cover image, date, title, preview text, reading time, category, "Read more"
- Detail view: full article
- Future-ready for: comments, search, categories, tags, related posts

### 2.3 Travel Guides Page
- Card grid: cover, country, city (optional), duration, description, difficulty (optional), free/premium badge, rating (future), download count (future)
- Filters: country, continent, free/premium
- **Guide Detail Modal:** large cover, full description, "what's included", sample pages, features, last updated date
  - **Free flow:** prominent download button → immediate download (future: optional email capture)
  - **Premium flow:** price + description + purchase button → checkout → collect email → auto-deliver guide → confirmation. Payment provider must be pluggable (start with Stripe).

### 2.4 Admin Panel (future phase, designed for now)
Manage: news, guides, images, homepage sections, collaborations, downloadable files, prices, featured content.

---

## 3. Acceptance Criteria

### Home Page
- [ ] Hero renders a full-bleed image/video background with headline, slogan, and ≥2 CTA buttons, responsive at 375px–2560px widths
- [ ] About section pulls copy and stats (countries visited, years traveling) from the API, not hardcoded
- [ ] Travel Highlights renders as a responsive grid/carousel of image cards with destination + caption
- [ ] Social section lists all 5 platforms with working outbound links (`target="_blank" rel="noopener"`), icon, description; follower count field exists in schema even if unused today
- [ ] Collaborations section renders logos + description from API; empty state hidden gracefully if no data
- [ ] Featured Guides shows a max of N (configurable) guides with correct free/premium badge and links to Guides page/detail
- [ ] News preview shows latest 3 articles sorted by publish date descending, "View all" links to `/news`
- [ ] Contact section displays business email (mailto link) and social icons; contact form is stubbed but not required for launch
- [ ] Page passes Lighthouse SEO ≥ 95, Performance ≥ 90 (mobile), Accessibility ≥ 95

### News Page
- [ ] `/news` lists all published articles as cards with cover, date, title, excerpt, reading time, category
- [ ] `/news/[slug]` renders full article with proper `<article>` semantics, OG tags, and JSON-LD `Article` schema
- [ ] Pagination or infinite scroll works correctly at boundary conditions (0 articles, 1 page, N pages)
- [ ] Draft/unpublished articles are never served on the public site

### Travel Guides Page
- [ ] `/guides` lists all published guides with working filters (country, continent, free/premium) that combine with AND logic
- [ ] Filter state is reflected in the URL query string (shareable/bookmarkable)
- [ ] Clicking a guide opens a modal (or dedicated route `/guides/[slug]` for SEO) with full detail content
- [ ] Free guide: clicking Download triggers an authenticated, rate-limited file download without requiring login
- [ ] Premium guide: Buy button launches checkout; on success, buyer email is captured, purchase record persisted, guide delivered (download link + emailed link), confirmation screen shown
- [ ] Failed/abandoned payments do not create a delivery record
- [ ] Guide detail pages are indexable (SSR/SSG) and have unique title/description/OG image per guide

### Cross-cutting
- [ ] Every page exposes editable `title`, `meta description`, and Open Graph fields via the API/CMS layer
- [ ] All images use responsive `srcset`/Next.js `<Image>` with lazy loading below the fold
- [ ] Site is fully navigable by keyboard; focus states visible; color contrast meets WCAG AA
- [ ] No content is hardcoded in the frontend beyond static UI copy (labels, buttons) — all business content comes from the API
- [ ] API responses are versioned (`/api/v1/...`) to allow non-breaking evolution

---

## 4. Technical Architecture

### 4.1 Backend — ASP.NET Core (.NET 8+)
- **Style:** Clean Architecture (Domain / Application / Infrastructure / API layers)
- **API:** REST, versioned (`/api/v1`), OpenAPI/Swagger documented
- **Auth:** Authentication-ready (ASP.NET Identity or JWT), not required for public read endpoints; required for future admin endpoints
- **Persistence:** EF Core + relational DB (PostgreSQL recommended for cloud portability, or SQL Server if preferred)
- **Storage:** Cloud object storage (Azure Blob / S3-compatible) for images and downloadable guide files, served via CDN
- **Payments:** Provider-agnostic `IPaymentProvider` abstraction; initial implementation with Stripe Checkout; webhook endpoint for payment confirmation
- **Email:** Transactional email abstraction (`IEmailSender`) — initial provider e.g. SendGrid/Resend — used for guide delivery and purchase confirmation
- **Background jobs:** Lightweight job/queue (Hangfire or simple hosted service) for post-payment guide delivery emails
- **Deployment:** Containerized (Docker), cloud-ready (Azure App Service / AWS / Fly.io)

**Core domain entities (initial):**
`Page`, `HomeSection`, `NewsArticle`, `Category`, `Tag`, `TravelGuide`, `GuideFile`, `Collaboration`, `SocialLink`, `Purchase`, `DownloadLog`, `MediaAsset`

### 4.2 Frontend — Next.js (React, App Router)
- **Rendering:** SSG for guides/news detail (ISR for updates), SSR/CSR where freshness matters (e.g., checkout)
- **Styling:** Tailwind CSS + a small design-token system (colors, spacing, type scale) for the premium travel aesthetic
- **Animation:** Framer Motion for scroll reveals/transitions, used sparingly
- **Images:** `next/image` with remote loader pointing at CDN/object storage
- **State/data fetching:** Server components + typed API client (generated from OpenAPI spec) for consistency with backend
- **SEO:** `next-seo` or App Router metadata API, per-page OG images, sitemap.xml + robots.txt auto-generated from API content
- **i18n (implemented):** Ukrainian (default) + English. Locale is held in a `locale` cookie (no URL segment) and toggled via a header switcher; server components read it with `getLocale()` and pass `?lang=` to the API. Static UI copy lives in `frontend/src/i18n/dictionaries.ts`; editable content is localized by the API.

### 4.3 API Contract Shape (illustrative)
```
GET  /api/v1/home                 -> hero, about, highlights, social, collaborations, featuredGuides, latestNews, contact
GET  /api/v1/news?page=&category= -> paginated NewsArticle[]
GET  /api/v1/news/{slug}
GET  /api/v1/guides?country=&continent=&type=
GET  /api/v1/guides/{slug}
POST /api/v1/guides/{slug}/download        (free guides)
POST /api/v1/guides/{slug}/checkout        (premium guides -> returns payment session)
POST /api/v1/payments/webhook              (provider callback -> fulfil purchase)
GET  /api/v1/collaborations
GET  /api/v1/social-links
```

### 4.4 Non-Functional Requirements
- Performance: Core Web Vitals green on mobile 4G throttling
- Accessibility: WCAG 2.1 AA, semantic HTML, visible focus states
- Security: input validation on all mutating endpoints, signed/expiring download URLs, webhook signature verification for payments
- SEO: unique metadata + OG per page, sitemap, structured data (Article, Product/Guide, Organization)

---

## 5. Implementation Plan

### Phase 0 — Foundation (1–2 weeks)
- Repo scaffolding: `backend/` (ASP.NET Core Clean Architecture solution) and `frontend/` (Next.js app)
- CI pipeline (build + lint + test) for both projects
- Base design system in Tailwind (colors, type scale, spacing, buttons, cards)
- Database schema + migrations for core entities
- Deployable "Hello World" for both apps to target cloud environment

### Phase 1 — Content Backbone
- Backend: CRUD for `NewsArticle`, `TravelGuide`, `Collaboration`, `SocialLink`, `HomeSection` (via seed data / simple admin scripts, full admin UI comes later)
- Frontend: Home page fully wired to API (Hero, About, Highlights, Social, Collaborations, Featured Guides, News preview, Contact)
- Image pipeline: upload → object storage → CDN URL stored on entities

### Phase 2 — News & Guides Public Pages
- `/news` list + `/news/[slug]` detail, SSG/ISR, SEO metadata, sitemap entry
- `/guides` list with filters, `/guides/[slug]` detail (modal + deep-linkable route)
- Free guide download flow with signed URLs + download logging

### Phase 3 — Monetization
- Stripe Checkout integration behind `IPaymentProvider`
- Purchase → webhook → email delivery pipeline (guide link + confirmation)
- Purchase confirmation UI + basic error/retry handling

### Phase 4 — Polish
- Animations (scroll reveals, hero transitions) via Framer Motion
- Accessibility audit + fixes
- Performance pass (image optimization, bundle analysis, Lighthouse targets)
- Cross-device QA (desktop, laptop, tablet, mobile)

### Phase 5 — Admin Panel (post-MVP)
- Authenticated admin app (could be a protected Next.js route group or separate app) for managing all content entities listed in section 2.4
- Role-based access if collaborators are added later

### Phase 6+ — Roadmap (not in MVP scope)
User accounts & saved favorites, booking integrations, interactive maps, newsletters, itineraries, additional digital products, affiliate links, sponsor/media-kit pages, analytics dashboard, AI travel assistant, comments, reviews, wishlist, travel planner.

> **Multi-language — delivered.** Ukrainian (default) + English across static UI copy and editable content. Each translatable entity field has a nullable `*En` sibling column (base column = Ukrainian); public read endpoints resolve `?lang=en` with fallback to the base value. The admin edits both languages side by side. Adding a third language later would mean either more sibling columns or a move to translation tables.

---

## 6. Decisions

| Question | Decision |
|---|---|
| Database | **PostgreSQL** — free, open-source, first-class EF Core support (Npgsql), runs free-tier on nearly every cloud (Azure, AWS RDS free tier, Neon, Supabase, Railway) and free locally via Docker |
| Cloud host | **Deferred** — not needed yet; architecture stays cloud-agnostic (Docker containers) so Azure/AWS/other can be decided later without rework |
| Payments | **Stripe** — Checkout + webhooks, behind the `IPaymentProvider` abstraction from section 4.1 |
| Email | **Resend** — has a generous free tier (3,000 emails/month, 100/day), simple API, good deliverability; used behind the `IEmailSender` abstraction so it can be swapped later if needed |
| Initial content | **Placeholder/seed data** — Phase 1 ships with realistic seed data (sample bio, stock-style travel photos, dummy collaborations/guides) so the full pipeline can be built and demoed before real content is supplied |

### Notes on the free-tier choices
- **PostgreSQL**: no licensing cost ever, unlike SQL Server which requires a paid tier beyond Express's size/feature limits for production use. Works identically in local Docker dev and in cloud-managed Postgres later.
- **Resend**: free tier is enough for guide-delivery + confirmation emails at low-to-moderate volume; SendGrid remains a drop-in alternative behind the same interface if volume grows or deliverability needs change.
- Both choices are wrapped in interfaces (`IEmailSender`, `IPaymentProvider`, EF Core provider abstraction) specifically so swapping providers later is a config change, not a rewrite.
