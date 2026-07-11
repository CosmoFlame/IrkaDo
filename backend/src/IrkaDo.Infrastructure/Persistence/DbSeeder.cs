using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Infrastructure.Persistence;

/// <summary>
/// Seeds the minimal scaffolding the site and admin need to function: the singleton home
/// sections (Hero / About / Contact), per-page SEO metadata, social links, and the news
/// taxonomy (categories / tags). All editorial content — news, guides, collaborations — is
/// added by the owner from the admin panel, so none of it is seeded here.
///
/// Idempotent: skipped entirely once any HomeSection row exists, since HomeSections are always
/// written last in the pass.
///
/// The site is bilingual: base columns hold the default language (Ukrainian) and the <c>*En</c>
/// siblings hold the English translation. Public read endpoints resolve <c>?lang=en</c> to the
/// English value and fall back to the Ukrainian base when a translation is absent.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db, IFileStorageService storage, CancellationToken cancellationToken = default)
    {
        if (await db.HomeSections.AnyAsync(cancellationToken))
            return;

        MediaAsset Photo(string key, string altUk, string altEn) => new()
        {
            Url = storage.GetPublicUrl($"media/{key}.jpg"),
            Type = MediaAssetType.Image,
            AltText = altUk,
            AltTextEn = altEn
        };

        var heroBg = Photo("hero-bg",
            "Ірина стоїть на скелі над океаном на заході сонця",
            "Iryna standing on a cliffside overlooking the ocean at sunset");
        var aboutPortrait = Photo("about-portrait",
            "Ірина Долженко усміхається з рюкзаком за плечима",
            "Iryna Dolzhenko smiling with a backpack");

        var homeSections = new[]
        {
            new HomeSection
            {
                Type = HomeSectionType.Hero,
                Headline = "Ловлю світанки на кожному континенті",
                HeadlineEn = "Chasing Sunrises on Every Continent",
                Body = "Я Ірина — тревел-креаторка, оповідачка й безнадійна мандрівниця. Приєднуйся, поки я " +
                       "досліджую найкрасивіші куточки світу й допомагаю тобі спланувати подорож мрії.",
                BodyEn = "I'm Iryna — travel creator, storyteller, and hopeless wanderer. Follow along as I " +
                         "explore the world's most beautiful places and help you plan the trip of a lifetime.",
                BackgroundMedia = heroBg
            },
            new HomeSection
            {
                Type = HomeSectionType.About,
                Headline = "Десять років, 60+ країн, одна сумка з камерою",
                HeadlineEn = "Ten Years, 60+ Countries, One Camera Bag",
                Body = "Те, що починалося як квиток в один бік із Києва, перетворилося на десять років подорожей " +
                       "у режимі нон-стоп. Я з рюкзаком підкоряла пустелі, ловила північне сяйво й ночувала " +
                       "будь-де — від нічних поїздів до п'ятизіркових курортів — і ділюся тут усім без прикрас. " +
                       "Моя місія проста: показати, що змістовні й красиві подорожі можливі майже за будь-якого " +
                       "бюджету, якщо знати, де шукати.",
                BodyEn = "What started as a one-way ticket out of Kyiv turned into a decade of full-time " +
                         "travel. I've backpacked deserts, chased the Northern Lights, and slept in everything " +
                         "from overnight trains to five-star resorts — and I share it all here, unfiltered. My " +
                         "mission is simple: show you that meaningful, beautiful travel is possible on almost " +
                         "any budget, if you know where to look.",
                BackgroundMedia = aboutPortrait
            },
            new HomeSection
            {
                Type = HomeSectionType.Contact,
                Headline = "Створімо щось неймовірне разом",
                HeadlineEn = "Let's Create Something Amazing Together",
                Body = "Щодо співпраці, партнерств із брендами чи запитів для преси — пиши будь-коли, я " +
                       "особисто читаю кожне повідомлення.",
                BodyEn = "For collaborations, brand partnerships, or press inquiries, reach out any time — " +
                         "I read every message myself.",
                ContactEmail = "hello@irkado.com"
            }
        };

        // Per-page SEO metadata, editable in the admin and consumed by the public pages' generateMetadata.
        var pages = new[]
        {
            new Page
            {
                Slug = "home",
                Title = "IrkaDo — тревел-креаторка Ірина Долженко",
                TitleEn = "IrkaDo — Travel Creator Iryna Dolzhenko",
                MetaTitle = "IrkaDo — тревел-креаторка Ірина Долженко",
                MetaTitleEn = "IrkaDo — Travel Creator Iryna Dolzhenko",
                MetaDescription = "Тревел-гайди, історії та натхнення для подорожей від Ірини Долженко (Irka_do).",
                MetaDescriptionEn = "Travel guides, stories, and inspiration from Iryna Dolzhenko (Irka_do)."
            },
            new Page
            {
                Slug = "news",
                Title = "Новини та історії",
                TitleEn = "News & Stories",
                MetaTitle = "Новини та історії — IrkaDo",
                MetaTitleEn = "News & Stories — IrkaDo",
                MetaDescription = "Останні новини, розповіді з подорожей і закулісся від Ірини Долженко.",
                MetaDescriptionEn = "The latest news, travel stories, and behind-the-scenes from Iryna Dolzhenko."
            },
            new Page
            {
                Slug = "guides",
                Title = "Тревел-гайди",
                TitleEn = "Travel Guides",
                MetaTitle = "Тревел-гайди — IrkaDo",
                MetaTitleEn = "Travel Guides — IrkaDo",
                MetaDescription = "Детальні маршрути та тревел-гайди по всьому світу — безкоштовні й преміум.",
                MetaDescriptionEn = "Detailed itineraries and travel guides from around the world — free and premium."
            }
        };

        var socialLinks = new[]
        {
            new SocialLink
            {
                Platform = SocialPlatform.Instagram, Url = "https://instagram.com/irka_do",
                Description = "Щоденні історії з дороги — моя найактивніша платформа.",
                DescriptionEn = "Daily stories from the road — my most active platform.",
                FollowerCount = 248_000, DisplayOrder = 0
            },
            new SocialLink
            {
                Platform = SocialPlatform.TikTok, Url = "https://tiktok.com/@irka_do",
                Description = "Швидкі лайфхаки для подорожей, поради зі збору валізи й огляди напрямків.",
                DescriptionEn = "Quick travel hacks, packing tips, and destination highlights.",
                FollowerCount = 182_000, DisplayOrder = 1
            },
            new SocialLink
            {
                Platform = SocialPlatform.YouTube, Url = "https://youtube.com/@irka_do",
                Description = "Довгі тревел-влоги та повні гайди по напрямках.",
                DescriptionEn = "Long-form travel vlogs and full destination guides.",
                FollowerCount = 94_000, DisplayOrder = 2
            },
            new SocialLink
            {
                Platform = SocialPlatform.Telegram, Url = "https://t.me/irka_do",
                Description = "Мій особистий щоденник подорожей і закулісні новини.",
                DescriptionEn = "My personal travel journal and behind-the-scenes updates.",
                FollowerCount = 15_000, DisplayOrder = 3
            },
            new SocialLink
            {
                Platform = SocialPlatform.Threads, Url = "https://threads.net/@irka_do",
                Description = "Короткі думки, опитування та Q&A зі спільнотою.",
                DescriptionEn = "Quick thoughts, polls, and community Q&As.",
                FollowerCount = 8_200, DisplayOrder = 4
            }
        };

        var categories = new[]
        {
            new Category { Name = "Напрямки", NameEn = "Destinations", Slug = "destinations" },
            new Category { Name = "Поради для подорожей", NameEn = "Travel Tips", Slug = "travel-tips" },
            new Category { Name = "За лаштунками", NameEn = "Behind the Scenes", Slug = "behind-the-scenes" },
            new Category { Name = "Спорядження", NameEn = "Gear", Slug = "gear" }
        };

        var tags = new[]
        {
            new Tag { Name = "Пригоди", NameEn = "Adventure", Slug = "adventure" },
            new Tag { Name = "Бюджетні подорожі", NameEn = "Budget Travel", Slug = "budget-travel" },
            new Tag { Name = "Соло-подорожі", NameEn = "Solo Travel", Slug = "solo-travel" },
            new Tag { Name = "Фотографія", NameEn = "Photography", Slug = "photography" },
            new Tag { Name = "Бекпекінг", NameEn = "Backpacking", Slug = "backpacking" },
            new Tag { Name = "Гурман", NameEn = "Foodie", Slug = "foodie" }
        };

        await db.SocialLinks.AddRangeAsync(socialLinks, cancellationToken);
        await db.Categories.AddRangeAsync(categories, cancellationToken);
        await db.Tags.AddRangeAsync(tags, cancellationToken);
        await db.HomeSections.AddRangeAsync(homeSections, cancellationToken);
        await db.Pages.AddRangeAsync(pages, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}
