using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Infrastructure.Persistence;

/// <summary>
/// Populates the database with realistic placeholder content (Phase 1 "content backbone") so the
/// full read pipeline can be built/demoed before real content is supplied. Idempotent: skipped
/// entirely once any HomeSection row exists, since HomeSections are always written last in the pass.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db, IFileStorageService storage, CancellationToken cancellationToken = default)
    {
        if (await db.HomeSections.AnyAsync(cancellationToken))
            return;

        MediaAsset Photo(string key, string alt) => new()
        {
            Url = storage.GetPublicUrl($"media/{key}.jpg"),
            Type = MediaAssetType.Image,
            AltText = alt
        };

        var heroBg = Photo("hero-bg", "Iryna standing on a cliffside overlooking the ocean at sunset");
        var aboutPortrait = Photo("about-portrait", "Iryna Dolzhenko smiling with a backpack");

        var highlightImages = new (string Key, string Destination, string Caption)[]
        {
            ("highlight-bali", "Ubud, Bali", "Sunrise hikes through rice terraces and a whole lot of coconut coffee."),
            ("highlight-santorini", "Santorini, Greece", "Blue domes, cliffside sunsets, and the best gyro of my life."),
            ("highlight-kyoto", "Kyoto, Japan", "Getting lost in bamboo forests and 400-year-old temples."),
            ("highlight-patagonia", "Patagonia, Argentina", "Ten days trekking through glaciers with no signal in sight."),
            ("highlight-marrakech", "Marrakech, Morocco", "Getting hopelessly (happily) lost in the medina's souks."),
            ("highlight-iceland", "Reykjavik, Iceland", "Chasing the Northern Lights until 3am in -10°C."),
        };

        var highlights = highlightImages.Select((h, i) => new TravelHighlight
        {
            Destination = h.Destination,
            Caption = h.Caption,
            DisplayOrder = i,
            IsPublished = true,
            Image = Photo(h.Key, h.Destination)
        }).ToArray();

        var homeSections = new[]
        {
            new HomeSection
            {
                Type = HomeSectionType.Hero,
                Headline = "Chasing Sunrises on Every Continent",
                Body = "I'm Iryna — travel creator, storyteller, and hopeless wanderer. Follow along as I " +
                       "explore the world's most beautiful places and help you plan the trip of a lifetime.",
                BackgroundMedia = heroBg
            },
            new HomeSection
            {
                Type = HomeSectionType.About,
                Headline = "Ten Years, 60+ Countries, One Camera Bag",
                Body = "What started as a one-way ticket out of Kyiv turned into a decade of full-time " +
                       "travel. I've backpacked deserts, chased the Northern Lights, and slept in everything " +
                       "from overnight trains to five-star resorts — and I share it all here, unfiltered. My " +
                       "mission is simple: show you that meaningful, beautiful travel is possible on almost " +
                       "any budget, if you know where to look.",
                BackgroundMedia = aboutPortrait
            },
            new HomeSection
            {
                Type = HomeSectionType.Contact,
                Headline = "Let's Create Something Amazing Together",
                Body = "For collaborations, brand partnerships, or press inquiries, reach out any time — " +
                       "I read every message myself."
            }
        };

        var socialLinks = new[]
        {
            new SocialLink
            {
                Platform = SocialPlatform.Instagram, Url = "https://instagram.com/irka_do",
                Description = "Daily stories from the road — my most active platform.",
                FollowerCount = 248_000, DisplayOrder = 0
            },
            new SocialLink
            {
                Platform = SocialPlatform.TikTok, Url = "https://tiktok.com/@irka_do",
                Description = "Quick travel hacks, packing tips, and destination highlights.",
                FollowerCount = 182_000, DisplayOrder = 1
            },
            new SocialLink
            {
                Platform = SocialPlatform.YouTube, Url = "https://youtube.com/@irka_do",
                Description = "Long-form travel vlogs and full destination guides.",
                FollowerCount = 94_000, DisplayOrder = 2
            },
            new SocialLink
            {
                Platform = SocialPlatform.Telegram, Url = "https://t.me/irka_do",
                Description = "My personal travel journal and behind-the-scenes updates.",
                FollowerCount = 15_000, DisplayOrder = 3
            },
            new SocialLink
            {
                Platform = SocialPlatform.Threads, Url = "https://threads.net/@irka_do",
                Description = "Quick thoughts, polls, and community Q&As.",
                FollowerCount = 8_200, DisplayOrder = 4
            }
        };

        var collaborations = new[]
        {
            new Collaboration
            {
                BrandName = "Skyline Airlines",
                Description = "Official travel partner for Southeast Asia flight routes since 2023.",
                Testimonial = "Iryna's content brought a level of authenticity to our campaign that " +
                               "traditional advertising just can't match. — Skyline Airlines Marketing Team",
                DisplayOrder = 0,
                IsPublished = true,
                Logo = Photo("collab-logo-skyline", "Skyline Airlines logo"),
                CampaignImages = [Photo("collab-campaign-skyline", "Skyline Airlines campaign photo")]
            },
            new Collaboration
            {
                BrandName = "Nomad Backpacks",
                Description = "Gear partner providing packs and luggage for every long-haul trip.",
                Testimonial = "Her real-world product reviews drove more engagement than any ad we've run. " +
                               "— Nomad Backpacks",
                DisplayOrder = 1,
                IsPublished = true,
                Logo = Photo("collab-logo-nomad", "Nomad Backpacks logo"),
                CampaignImages = [Photo("collab-campaign-nomad", "Nomad Backpacks campaign photo")]
            },
            new Collaboration
            {
                BrandName = "Aurora Hotels",
                Description = "Featured stays across Iceland and Scandinavia's Aurora Hotels collection.",
                DisplayOrder = 2,
                IsPublished = true,
                Logo = Photo("collab-logo-aurora", "Aurora Hotels logo"),
                CampaignImages = [Photo("collab-campaign-aurora", "Aurora Hotels campaign photo")]
            }
        };

        var categories = new[]
        {
            new Category { Name = "Destinations", Slug = "destinations" },
            new Category { Name = "Travel Tips", Slug = "travel-tips" },
            new Category { Name = "Behind the Scenes", Slug = "behind-the-scenes" },
            new Category { Name = "Gear", Slug = "gear" }
        };
        var destinationsCategory = categories[0];
        var travelTipsCategory = categories[1];
        var behindTheScenesCategory = categories[2];
        var gearCategory = categories[3];

        var tags = new[]
        {
            new Tag { Name = "Adventure", Slug = "adventure" },
            new Tag { Name = "Budget Travel", Slug = "budget-travel" },
            new Tag { Name = "Solo Travel", Slug = "solo-travel" },
            new Tag { Name = "Photography", Slug = "photography" },
            new Tag { Name = "Backpacking", Slug = "backpacking" },
            new Tag { Name = "Foodie", Slug = "foodie" }
        };
        var adventureTag = tags[0];
        var budgetTravelTag = tags[1];
        var soloTravelTag = tags[2];
        var photographyTag = tags[3];
        var foodieTag = tags[5];

        var baliGuide = new TravelGuide
        {
            Title = "Bali Escape",
            Slug = "bali-escape",
            Country = "Indonesia",
            City = "Ubud",
            Continent = "Asia",
            Description = "A 7-day guide to Bali's best rice terraces, waterfalls, and beach towns — " +
                          "built from three separate trips.",
            WhatsIncluded = "Day-by-day itinerary, budget breakdown, map of hidden spots, restaurant recommendations.",
            DurationDays = 7,
            Difficulty = GuideDifficulty.Easy,
            IsPremium = false,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-14),
            CoverImage = Photo("guide-cover-bali", "Rice terraces in Ubud, Bali"),
            MetaTitle = "Bali Escape — Free 7-Day Travel Guide",
            MetaDescription = "A free, detailed 7-day itinerary for Bali covering Ubud, rice terraces, and hidden beaches."
        };
        baliGuide.Files.Add(new GuideFile
        {
            FileName = "bali-escape-guide.pdf",
            StorageKey = "bali-escape-guide.pdf",
            SizeBytes = 506
        });

        var japanGuide = new TravelGuide
        {
            Title = "Japan in Two Weeks",
            Slug = "japan-in-two-weeks",
            Country = "Japan",
            City = "Tokyo",
            Continent = "Asia",
            Description = "The exact 14-day route I used to cover Tokyo, Kyoto, and Osaka without rushing.",
            WhatsIncluded = "Full itinerary, JR Pass advice, restaurant map, packing list, budget breakdown.",
            DurationDays = 14,
            Difficulty = GuideDifficulty.Moderate,
            IsPremium = true,
            PriceAmount = 19.99m,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-7),
            CoverImage = Photo("guide-cover-japan", "Street in Tokyo, Japan"),
            PreviewImages = [Photo("guide-preview-japan", "Preview page of the Japan travel guide")],
            MetaTitle = "Japan in Two Weeks — Premium Travel Guide",
            MetaDescription = "A detailed 14-day Japan itinerary covering Tokyo, Kyoto, and Osaka."
        };
        japanGuide.Files.Add(new GuideFile
        {
            FileName = "japan-in-two-weeks-guide.pdf",
            StorageKey = "japan-in-two-weeks-guide.pdf",
            SizeBytes = 512
        });

        var patagoniaGuide = new TravelGuide
        {
            Title = "Patagonia Trekking Guide",
            Slug = "patagonia-trekking-guide",
            Country = "Argentina",
            City = "El Chaltén",
            Continent = "South America",
            Description = "Everything I wish I'd known before ten days of trekking through Los Glaciares National Park.",
            WhatsIncluded = "Trail-by-trail breakdown, gear checklist, weather advice, campsite map.",
            DurationDays = 10,
            Difficulty = GuideDifficulty.Challenging,
            IsPremium = true,
            PriceAmount = 24.99m,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-21),
            CoverImage = Photo("guide-cover-patagonia", "Glacier trekking trail in Patagonia"),
            PreviewImages = [Photo("guide-preview-patagonia", "Preview page of the Patagonia travel guide")],
            MetaTitle = "Patagonia Trekking Guide — Premium Travel Guide",
            MetaDescription = "A detailed 10-day trekking guide to Los Glaciares National Park, Patagonia."
        };
        patagoniaGuide.Files.Add(new GuideFile
        {
            FileName = "patagonia-trekking-guide.pdf",
            StorageKey = "patagonia-trekking-guide.pdf",
            SizeBytes = 512
        });

        var santoriniGuide = new TravelGuide
        {
            Title = "Santorini Weekend",
            Slug = "santorini-weekend",
            Country = "Greece",
            City = "Santorini",
            Continent = "Europe",
            Description = "A relaxed 4-day itinerary for Santorini's best sunset spots without the tourist crowds.",
            WhatsIncluded = "Day-by-day itinerary, restaurant map, budget breakdown.",
            DurationDays = 4,
            Difficulty = GuideDifficulty.Easy,
            IsPremium = false,
            IsPublished = true,
            IsFeatured = false,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            CoverImage = Photo("guide-cover-santorini", "Blue-domed church in Santorini, Greece"),
            MetaTitle = "Santorini Weekend — Free 4-Day Travel Guide",
            MetaDescription = "A free 4-day Santorini itinerary covering the best sunset spots and local food."
        };
        santoriniGuide.Files.Add(new GuideFile
        {
            FileName = "santorini-weekend-guide.pdf",
            StorageKey = "santorini-weekend-guide.pdf",
            SizeBytes = 512
        });

        var moroccoGuide = new TravelGuide
        {
            Title = "Morocco Desert Adventure",
            Slug = "morocco-desert-adventure",
            Country = "Morocco",
            City = "Marrakech",
            Continent = "Africa",
            Description = "An 8-day loop from Marrakech's medina to camping under the stars in the Sahara.",
            WhatsIncluded = "Day-by-day itinerary, desert camp recommendations, packing list.",
            DurationDays = 8,
            Difficulty = GuideDifficulty.Moderate,
            IsPremium = true,
            PriceAmount = 17.99m,
            IsPublished = true,
            IsFeatured = false,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-45),
            CoverImage = Photo("guide-cover-morocco", "Sahara desert dunes near Marrakech, Morocco"),
            MetaTitle = "Morocco Desert Adventure — Premium Travel Guide",
            MetaDescription = "An 8-day Morocco itinerary from the Marrakech medina to Sahara desert camps."
        };
        moroccoGuide.Files.Add(new GuideFile
        {
            FileName = "morocco-desert-adventure-guide.pdf",
            StorageKey = "morocco-desert-adventure-guide.pdf",
            SizeBytes = 512
        });

        var guides = new[] { baliGuide, japanGuide, patagoniaGuide, santoriniGuide, moroccoGuide };

        var newsArticles = new[]
        {
            new NewsArticle
            {
                Title = "5 Reasons to Visit Bali This Year",
                Slug = "5-reasons-to-visit-bali-this-year",
                Excerpt = "From quiet rice terraces to buzzing beach towns, here's why Bali deserves a spot on your list.",
                Content = "<p>From quiet rice terraces to buzzing beach towns, here's why Bali deserves a spot " +
                          "on your list this year — and how to see it without the crowds.</p>",
                ReadingTimeMinutes = 4,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-3),
                CoverImage = Photo("news-cover-bali", "Rice terraces in Bali"),
                Category = destinationsCategory,
                Tags = [adventureTag, budgetTravelTag],
                MetaTitle = "5 Reasons to Visit Bali This Year"
            },
            new NewsArticle
            {
                Title = "How I Planned a 2-Week Japan Itinerary",
                Slug = "how-i-planned-a-2-week-japan-itinerary",
                Excerpt = "The exact planning process, budget, and mistakes I'd avoid next time.",
                Content = "<p>The exact planning process, budget, and mistakes I'd avoid next time — a full " +
                          "breakdown of how a 14-day Japan trip actually came together.</p>",
                ReadingTimeMinutes = 6,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-10),
                CoverImage = Photo("news-cover-japan", "Street in Tokyo"),
                Category = travelTipsCategory,
                Tags = [soloTravelTag],
                MetaTitle = "How I Planned a 2-Week Japan Itinerary"
            },
            new NewsArticle
            {
                Title = "Behind the Scenes: Filming in Patagonia",
                Slug = "behind-the-scenes-filming-in-patagonia",
                Excerpt = "What it actually took to film ten days of trekking footage in freezing wind.",
                Content = "<p>What it actually took to film ten days of trekking footage in freezing wind — " +
                          "gear failures included.</p>",
                ReadingTimeMinutes = 5,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-20),
                CoverImage = Photo("news-cover-patagonia", "Trekking trail in Patagonia"),
                Category = behindTheScenesCategory,
                Tags = [adventureTag, photographyTag],
                MetaTitle = "Behind the Scenes: Filming in Patagonia"
            },
            new NewsArticle
            {
                Title = "My Top Camera Gear for Travel Content",
                Slug = "my-top-camera-gear-for-travel-content",
                Excerpt = "The exact kit I pack for every trip, and what I've stopped bringing.",
                Content = "<p>The exact kit I pack for every trip, and what I've stopped bringing after ten " +
                          "years on the road.</p>",
                ReadingTimeMinutes = 7,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-35),
                CoverImage = Photo("news-cover-gear", "Camera gear laid out for travel"),
                Category = gearCategory,
                Tags = [photographyTag],
                MetaTitle = "My Top Camera Gear for Travel Content"
            },
            new NewsArticle
            {
                Title = "A Love Letter to Santorini Sunsets",
                Slug = "a-love-letter-to-santorini-sunsets",
                Excerpt = "Why I keep going back to the same clifftop town, year after year.",
                Content = "<p>Why I keep going back to the same clifftop town, year after year — and where " +
                          "to watch the sunset without the tour bus crowds.</p>",
                ReadingTimeMinutes = 3,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-50),
                CoverImage = Photo("news-cover-santorini", "Sunset over Santorini"),
                Category = destinationsCategory,
                Tags = [foodieTag],
                MetaTitle = "A Love Letter to Santorini Sunsets"
            }
        };

        await db.TravelHighlights.AddRangeAsync(highlights, cancellationToken);
        await db.SocialLinks.AddRangeAsync(socialLinks, cancellationToken);
        await db.Collaborations.AddRangeAsync(collaborations, cancellationToken);
        await db.Categories.AddRangeAsync(categories, cancellationToken);
        await db.Tags.AddRangeAsync(tags, cancellationToken);
        await db.TravelGuides.AddRangeAsync(guides, cancellationToken);
        await db.NewsArticles.AddRangeAsync(newsArticles, cancellationToken);
        await db.HomeSections.AddRangeAsync(homeSections, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}
