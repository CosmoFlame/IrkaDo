using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Infrastructure.Persistence;

/// <summary>
/// Populates the database with realistic placeholder content (Phase 1 "content backbone") so the
/// full read pipeline can be built/demoed before real content is supplied. Idempotent: skipped
/// entirely once any HomeSection row exists, since HomeSections are always written last in the pass.
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

        var highlightImages = new (string Key, string DestUk, string DestEn, string CaptionUk, string CaptionEn)[]
        {
            ("highlight-bali", "Убуд, Балі", "Ubud, Bali",
                "Світанкові походи рисовими терасами й безмежна кокосова кава.",
                "Sunrise hikes through rice terraces and a whole lot of coconut coffee."),
            ("highlight-santorini", "Санторіні, Греція", "Santorini, Greece",
                "Блакитні куполи, заходи сонця над скелями й найкращий гірос у моєму житті.",
                "Blue domes, cliffside sunsets, and the best gyro of my life."),
            ("highlight-kyoto", "Кіото, Японія", "Kyoto, Japan",
                "Загубитися в бамбукових лісах і 400-річних храмах.",
                "Getting lost in bamboo forests and 400-year-old temples."),
            ("highlight-patagonia", "Патагонія, Аргентина", "Patagonia, Argentina",
                "Десять днів треку серед льодовиків без жодного сигналу зв'язку.",
                "Ten days trekking through glaciers with no signal in sight."),
            ("highlight-marrakech", "Марракеш, Марокко", "Marrakech, Morocco",
                "Безнадійно (і щасливо) загубитися в базарах медіни.",
                "Getting hopelessly (happily) lost in the medina's souks."),
            ("highlight-iceland", "Рейк'явік, Ісландія", "Reykjavik, Iceland",
                "Полювати на північне сяйво до 3-ї ночі за −10 °C.",
                "Chasing the Northern Lights until 3am in -10°C."),
        };

        var highlights = highlightImages.Select((h, i) => new TravelHighlight
        {
            Destination = h.DestUk,
            DestinationEn = h.DestEn,
            Caption = h.CaptionUk,
            CaptionEn = h.CaptionEn,
            DisplayOrder = i,
            IsPublished = true,
            Image = Photo(h.Key, h.DestUk, h.DestEn)
        }).ToArray();

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
                MetaDescription = "Тревел-гіди, історії та натхнення для подорожей від Ірини Долженко (Irka_do).",
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
                Title = "Тревел-гіди",
                TitleEn = "Travel Guides",
                MetaTitle = "Тревел-гіди — IrkaDo",
                MetaTitleEn = "Travel Guides — IrkaDo",
                MetaDescription = "Детальні маршрути та тревел-гіди по всьому світу — безкоштовні й преміум.",
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
                Description = "Довгі тревел-влоги та повні гіди по напрямках.",
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

        var collaborations = new[]
        {
            new Collaboration
            {
                BrandName = "Skyline Airlines",
                Description = "Офіційний тревел-партнер на авіанапрямках Південно-Східної Азії з 2023 року.",
                DescriptionEn = "Official travel partner for Southeast Asia flight routes since 2023.",
                Testimonial = "Контент Ірини додав нашій кампанії рівня автентичності, якого традиційна " +
                              "реклама просто не може дати. — Маркетингова команда Skyline Airlines",
                TestimonialEn = "Iryna's content brought a level of authenticity to our campaign that " +
                                "traditional advertising just can't match. — Skyline Airlines Marketing Team",
                DisplayOrder = 0,
                IsPublished = true,
                Logo = Photo("collab-logo-skyline", "Логотип Skyline Airlines", "Skyline Airlines logo"),
                CampaignImages = [Photo("collab-campaign-skyline", "Кампанійне фото Skyline Airlines", "Skyline Airlines campaign photo")]
            },
            new Collaboration
            {
                BrandName = "Nomad Backpacks",
                Description = "Партнер зі спорядження, що надає рюкзаки й багаж для кожної далекої подорожі.",
                DescriptionEn = "Gear partner providing packs and luggage for every long-haul trip.",
                Testimonial = "Її реальні огляди продуктів дали більше залученості, ніж будь-яка наша реклама. " +
                              "— Nomad Backpacks",
                TestimonialEn = "Her real-world product reviews drove more engagement than any ad we've run. " +
                                "— Nomad Backpacks",
                DisplayOrder = 1,
                IsPublished = true,
                Logo = Photo("collab-logo-nomad", "Логотип Nomad Backpacks", "Nomad Backpacks logo"),
                CampaignImages = [Photo("collab-campaign-nomad", "Кампанійне фото Nomad Backpacks", "Nomad Backpacks campaign photo")]
            },
            new Collaboration
            {
                BrandName = "Aurora Hotels",
                Description = "Обрані готелі колекції Aurora Hotels по всій Ісландії та Скандинавії.",
                DescriptionEn = "Featured stays across Iceland and Scandinavia's Aurora Hotels collection.",
                DisplayOrder = 2,
                IsPublished = true,
                Logo = Photo("collab-logo-aurora", "Логотип Aurora Hotels", "Aurora Hotels logo"),
                CampaignImages = [Photo("collab-campaign-aurora", "Кампанійне фото Aurora Hotels", "Aurora Hotels campaign photo")]
            }
        };

        var categories = new[]
        {
            new Category { Name = "Напрямки", NameEn = "Destinations", Slug = "destinations" },
            new Category { Name = "Поради для подорожей", NameEn = "Travel Tips", Slug = "travel-tips" },
            new Category { Name = "За лаштунками", NameEn = "Behind the Scenes", Slug = "behind-the-scenes" },
            new Category { Name = "Спорядження", NameEn = "Gear", Slug = "gear" }
        };
        var destinationsCategory = categories[0];
        var travelTipsCategory = categories[1];
        var behindTheScenesCategory = categories[2];
        var gearCategory = categories[3];

        var tags = new[]
        {
            new Tag { Name = "Пригоди", NameEn = "Adventure", Slug = "adventure" },
            new Tag { Name = "Бюджетні подорожі", NameEn = "Budget Travel", Slug = "budget-travel" },
            new Tag { Name = "Соло-подорожі", NameEn = "Solo Travel", Slug = "solo-travel" },
            new Tag { Name = "Фотографія", NameEn = "Photography", Slug = "photography" },
            new Tag { Name = "Бекпекінг", NameEn = "Backpacking", Slug = "backpacking" },
            new Tag { Name = "Гурман", NameEn = "Foodie", Slug = "foodie" }
        };
        var adventureTag = tags[0];
        var budgetTravelTag = tags[1];
        var soloTravelTag = tags[2];
        var photographyTag = tags[3];
        var foodieTag = tags[5];

        var baliGuide = new TravelGuide
        {
            Title = "Втеча на Балі",
            TitleEn = "Bali Escape",
            Slug = "bali-escape",
            Country = "Індонезія",
            CountryEn = "Indonesia",
            City = "Убуд",
            CityEn = "Ubud",
            Continent = "Азія",
            ContinentEn = "Asia",
            Description = "7-денний гід найкращими рисовими терасами, водоспадами й пляжними містечками Балі — " +
                          "зібраний із трьох окремих поїздок.",
            DescriptionEn = "A 7-day guide to Bali's best rice terraces, waterfalls, and beach towns — " +
                            "built from three separate trips.",
            WhatsIncluded = "Покроковий маршрут, розбір бюджету, мапа прихованих місць, рекомендації ресторанів.",
            WhatsIncludedEn = "Day-by-day itinerary, budget breakdown, map of hidden spots, restaurant recommendations.",
            DurationDays = 7,
            Difficulty = GuideDifficulty.Easy,
            IsPremium = false,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-14),
            CoverImage = Photo("guide-cover-bali", "Рисові тераси в Убуді, Балі", "Rice terraces in Ubud, Bali"),
            MetaTitle = "Втеча на Балі — безкоштовний 7-денний гід",
            MetaTitleEn = "Bali Escape — Free 7-Day Travel Guide",
            MetaDescription = "Безкоштовний детальний 7-денний маршрут по Балі: Убуд, рисові тераси та приховані пляжі.",
            MetaDescriptionEn = "A free, detailed 7-day itinerary for Bali covering Ubud, rice terraces, and hidden beaches."
        };
        baliGuide.Files.Add(new GuideFile
        {
            FileName = "bali-escape-guide.pdf",
            StorageKey = "bali-escape-guide.pdf",
            SizeBytes = 506
        });

        var japanGuide = new TravelGuide
        {
            Title = "Японія за два тижні",
            TitleEn = "Japan in Two Weeks",
            Slug = "japan-in-two-weeks",
            Country = "Японія",
            CountryEn = "Japan",
            City = "Токіо",
            CityEn = "Tokyo",
            Continent = "Азія",
            ContinentEn = "Asia",
            Description = "Точний 14-денний маршрут, яким я охопила Токіо, Кіото й Осаку без поспіху.",
            DescriptionEn = "The exact 14-day route I used to cover Tokyo, Kyoto, and Osaka without rushing.",
            WhatsIncluded = "Повний маршрут, поради щодо JR Pass, мапа ресторанів, список речей, розбір бюджету.",
            WhatsIncludedEn = "Full itinerary, JR Pass advice, restaurant map, packing list, budget breakdown.",
            DurationDays = 14,
            Difficulty = GuideDifficulty.Moderate,
            IsPremium = true,
            PriceAmount = 19.99m,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-7),
            CoverImage = Photo("guide-cover-japan", "Вулиця в Токіо, Японія", "Street in Tokyo, Japan"),
            PreviewImages = [Photo("guide-preview-japan", "Сторінка-прев'ю гіда по Японії", "Preview page of the Japan travel guide")],
            MetaTitle = "Японія за два тижні — преміум-гід",
            MetaTitleEn = "Japan in Two Weeks — Premium Travel Guide",
            MetaDescription = "Детальний 14-денний маршрут по Японії: Токіо, Кіото та Осака.",
            MetaDescriptionEn = "A detailed 14-day Japan itinerary covering Tokyo, Kyoto, and Osaka."
        };
        japanGuide.Files.Add(new GuideFile
        {
            FileName = "japan-in-two-weeks-guide.pdf",
            StorageKey = "japan-in-two-weeks-guide.pdf",
            SizeBytes = 512
        });

        var patagoniaGuide = new TravelGuide
        {
            Title = "Гід треками Патагонії",
            TitleEn = "Patagonia Trekking Guide",
            Slug = "patagonia-trekking-guide",
            Country = "Аргентина",
            CountryEn = "Argentina",
            City = "Ель-Чальтен",
            CityEn = "El Chaltén",
            Continent = "Південна Америка",
            ContinentEn = "South America",
            Description = "Усе, що я хотіла б знати перед десятьма днями треку національним парком Лос-Ґласьярес.",
            DescriptionEn = "Everything I wish I'd known before ten days of trekking through Los Glaciares National Park.",
            WhatsIncluded = "Розбір кожного маршруту, чек-лист спорядження, поради щодо погоди, мапа кемпінгів.",
            WhatsIncludedEn = "Trail-by-trail breakdown, gear checklist, weather advice, campsite map.",
            DurationDays = 10,
            Difficulty = GuideDifficulty.Challenging,
            IsPremium = true,
            PriceAmount = 24.99m,
            IsPublished = true,
            IsFeatured = true,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-21),
            CoverImage = Photo("guide-cover-patagonia", "Трек льодовиком у Патагонії", "Glacier trekking trail in Patagonia"),
            PreviewImages = [Photo("guide-preview-patagonia", "Сторінка-прев'ю гіда по Патагонії", "Preview page of the Patagonia travel guide")],
            MetaTitle = "Гід треками Патагонії — преміум-гід",
            MetaTitleEn = "Patagonia Trekking Guide — Premium Travel Guide",
            MetaDescription = "Детальний 10-денний гід треками національного парку Лос-Ґласьярес, Патагонія.",
            MetaDescriptionEn = "A detailed 10-day trekking guide to Los Glaciares National Park, Patagonia."
        };
        patagoniaGuide.Files.Add(new GuideFile
        {
            FileName = "patagonia-trekking-guide.pdf",
            StorageKey = "patagonia-trekking-guide.pdf",
            SizeBytes = 512
        });

        var santoriniGuide = new TravelGuide
        {
            Title = "Вікенд на Санторіні",
            TitleEn = "Santorini Weekend",
            Slug = "santorini-weekend",
            Country = "Греція",
            CountryEn = "Greece",
            City = "Санторіні",
            CityEn = "Santorini",
            Continent = "Європа",
            ContinentEn = "Europe",
            Description = "Розслаблений 4-денний маршрут найкращими місцями для заходу сонця на Санторіні без натовпів.",
            DescriptionEn = "A relaxed 4-day itinerary for Santorini's best sunset spots without the tourist crowds.",
            WhatsIncluded = "Покроковий маршрут, мапа ресторанів, розбір бюджету.",
            WhatsIncludedEn = "Day-by-day itinerary, restaurant map, budget breakdown.",
            DurationDays = 4,
            Difficulty = GuideDifficulty.Easy,
            IsPremium = false,
            IsPublished = true,
            IsFeatured = false,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            CoverImage = Photo("guide-cover-santorini", "Церква з блакитним куполом на Санторіні, Греція", "Blue-domed church in Santorini, Greece"),
            MetaTitle = "Вікенд на Санторіні — безкоштовний 4-денний гід",
            MetaTitleEn = "Santorini Weekend — Free 4-Day Travel Guide",
            MetaDescription = "Безкоштовний 4-денний маршрут по Санторіні: найкращі заходи сонця та місцева кухня.",
            MetaDescriptionEn = "A free 4-day Santorini itinerary covering the best sunset spots and local food."
        };
        santoriniGuide.Files.Add(new GuideFile
        {
            FileName = "santorini-weekend-guide.pdf",
            StorageKey = "santorini-weekend-guide.pdf",
            SizeBytes = 512
        });

        var moroccoGuide = new TravelGuide
        {
            Title = "Пустельна пригода в Марокко",
            TitleEn = "Morocco Desert Adventure",
            Slug = "morocco-desert-adventure",
            Country = "Марокко",
            CountryEn = "Morocco",
            City = "Марракеш",
            CityEn = "Marrakech",
            Continent = "Африка",
            ContinentEn = "Africa",
            Description = "8-денне коло від медіни Марракеша до ночівлі під зорями в Сахарі.",
            DescriptionEn = "An 8-day loop from Marrakech's medina to camping under the stars in the Sahara.",
            WhatsIncluded = "Покроковий маршрут, рекомендації пустельних кемпів, список речей.",
            WhatsIncludedEn = "Day-by-day itinerary, desert camp recommendations, packing list.",
            DurationDays = 8,
            Difficulty = GuideDifficulty.Moderate,
            IsPremium = true,
            PriceAmount = 17.99m,
            IsPublished = true,
            IsFeatured = false,
            LastUpdatedAt = DateTimeOffset.UtcNow.AddDays(-45),
            CoverImage = Photo("guide-cover-morocco", "Дюни Сахари поблизу Марракеша, Марокко", "Sahara desert dunes near Marrakech, Morocco"),
            MetaTitle = "Пустельна пригода в Марокко — преміум-гід",
            MetaTitleEn = "Morocco Desert Adventure — Premium Travel Guide",
            MetaDescription = "8-денний маршрут по Марокко: від медіни Марракеша до пустельних кемпів Сахари.",
            MetaDescriptionEn = "An 8-day Morocco itinerary from the Marrakech medina to Sahara desert camps."
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
                Title = "5 причин відвідати Балі цього року",
                TitleEn = "5 Reasons to Visit Bali This Year",
                Slug = "5-reasons-to-visit-bali-this-year",
                Excerpt = "Від тихих рисових терас до жвавих пляжних містечок — ось чому Балі варте місця у твоєму списку.",
                ExcerptEn = "From quiet rice terraces to buzzing beach towns, here's why Bali deserves a spot on your list.",
                Content = "<p>Від тихих рисових терас до жвавих пляжних містечок — ось чому Балі варте місця у " +
                          "твоєму списку цього року, і як побачити його без натовпів.</p>",
                ContentEn = "<p>From quiet rice terraces to buzzing beach towns, here's why Bali deserves a spot " +
                            "on your list this year — and how to see it without the crowds.</p>",
                ReadingTimeMinutes = 4,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-3),
                CoverImage = Photo("news-cover-bali", "Рисові тераси на Балі", "Rice terraces in Bali"),
                Category = destinationsCategory,
                Tags = [adventureTag, budgetTravelTag],
                MetaTitle = "5 причин відвідати Балі цього року",
                MetaTitleEn = "5 Reasons to Visit Bali This Year"
            },
            new NewsArticle
            {
                Title = "Як я спланувала 2-тижневий маршрут по Японії",
                TitleEn = "How I Planned a 2-Week Japan Itinerary",
                Slug = "how-i-planned-a-2-week-japan-itinerary",
                Excerpt = "Точний процес планування, бюджет і помилки, яких я б уникла наступного разу.",
                ExcerptEn = "The exact planning process, budget, and mistakes I'd avoid next time.",
                Content = "<p>Точний процес планування, бюджет і помилки, яких я б уникла наступного разу — " +
                          "повний розбір того, як насправді склалася 14-денна подорож Японією.</p>",
                ContentEn = "<p>The exact planning process, budget, and mistakes I'd avoid next time — a full " +
                            "breakdown of how a 14-day Japan trip actually came together.</p>",
                ReadingTimeMinutes = 6,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-10),
                CoverImage = Photo("news-cover-japan", "Вулиця в Токіо", "Street in Tokyo"),
                Category = travelTipsCategory,
                Tags = [soloTravelTag],
                MetaTitle = "Як я спланувала 2-тижневий маршрут по Японії",
                MetaTitleEn = "How I Planned a 2-Week Japan Itinerary"
            },
            new NewsArticle
            {
                Title = "За лаштунками: зйомки в Патагонії",
                TitleEn = "Behind the Scenes: Filming in Patagonia",
                Slug = "behind-the-scenes-filming-in-patagonia",
                Excerpt = "Чого насправді коштувало відзняти десять днів треку на крижаному вітрі.",
                ExcerptEn = "What it actually took to film ten days of trekking footage in freezing wind.",
                Content = "<p>Чого насправді коштувало відзняти десять днів треку на крижаному вітрі — " +
                          "разом із поломками спорядження.</p>",
                ContentEn = "<p>What it actually took to film ten days of trekking footage in freezing wind — " +
                            "gear failures included.</p>",
                ReadingTimeMinutes = 5,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-20),
                CoverImage = Photo("news-cover-patagonia", "Трекінговий шлях у Патагонії", "Trekking trail in Patagonia"),
                Category = behindTheScenesCategory,
                Tags = [adventureTag, photographyTag],
                MetaTitle = "За лаштунками: зйомки в Патагонії",
                MetaTitleEn = "Behind the Scenes: Filming in Patagonia"
            },
            new NewsArticle
            {
                Title = "Моє найкраще спорядження для тревел-контенту",
                TitleEn = "My Top Camera Gear for Travel Content",
                Slug = "my-top-camera-gear-for-travel-content",
                Excerpt = "Точний набір, який я беру в кожну поїздку, і що я перестала брати.",
                ExcerptEn = "The exact kit I pack for every trip, and what I've stopped bringing.",
                Content = "<p>Точний набір, який я беру в кожну поїздку, і що я перестала брати за десять " +
                          "років у дорозі.</p>",
                ContentEn = "<p>The exact kit I pack for every trip, and what I've stopped bringing after ten " +
                            "years on the road.</p>",
                ReadingTimeMinutes = 7,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-35),
                CoverImage = Photo("news-cover-gear", "Камерне спорядження, розкладене для подорожі", "Camera gear laid out for travel"),
                Category = gearCategory,
                Tags = [photographyTag],
                MetaTitle = "Моє найкраще спорядження для тревел-контенту",
                MetaTitleEn = "My Top Camera Gear for Travel Content"
            },
            new NewsArticle
            {
                Title = "Лист любові до заходів сонця на Санторіні",
                TitleEn = "A Love Letter to Santorini Sunsets",
                Slug = "a-love-letter-to-santorini-sunsets",
                Excerpt = "Чому я знову й знову повертаюся до того самого містечка на скелі, рік за роком.",
                ExcerptEn = "Why I keep going back to the same clifftop town, year after year.",
                Content = "<p>Чому я знову й знову повертаюся до того самого містечка на скелі, рік за роком — " +
                          "і де спостерігати захід сонця без натовпу з туристичних автобусів.</p>",
                ContentEn = "<p>Why I keep going back to the same clifftop town, year after year — and where " +
                            "to watch the sunset without the tour bus crowds.</p>",
                ReadingTimeMinutes = 3,
                IsPublished = true,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-50),
                CoverImage = Photo("news-cover-santorini", "Захід сонця над Санторіні", "Sunset over Santorini"),
                Category = destinationsCategory,
                Tags = [foodieTag],
                MetaTitle = "Лист любові до заходів сонця на Санторіні",
                MetaTitleEn = "A Love Letter to Santorini Sunsets"
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
        await db.Pages.AddRangeAsync(pages, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}
