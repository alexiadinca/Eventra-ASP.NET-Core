using Eventra.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();
            await SeedUsers(context);
            await SeedPendingOrganizerRequest(context);
            await SeedCategories(context);
            await SeedEvents(context);
            await SeedParticipations(context);
            await SeedWaitingList(context);
            await SeedReviews(context);
            await SeedFavorites(context);
            await SeedNotifications(context);
        }

        // USERS 

        private static async Task SeedUsers(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<User>();

            if (!await context.Users.AnyAsync(u => u.Email == "admin@eventra.com"))
            {
                var u = new User
                {
                    FirstName = "Admin", LastName = "Eventra",
                    Email = "admin@eventra.com", Username = "admin",
                    Role = "Admin", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Admin2026");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "EventraStudios"))
            {
                var u = new User
                {
                    FirstName = "Eventra", LastName = "Studios",
                    Email = "eventra@gmail.com", Username = "EventraStudios",
                    Role = "Organizer", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png",
                    ProfilePhotoPath = "/uploads/profile-photos/b3627aa5-1cc6-434e-87b1-0c2961fdd95a.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Eventra1");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "TheLobbyRestaurant"))
            {
                var u = new User
                {
                    FirstName = "The Lobby", LastName = "Restaurant",
                    Email = "thelobby@gmail.com", Username = "TheLobbyRestaurant",
                    Role = "Organizer", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png",
                    ProfilePhotoPath = "/uploads/profile-photos/d8821eed-cd6f-482b-bea2-8e3ee436394a.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Thelobby1");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "Mayfair39"))
            {
                var u = new User
                {
                    FirstName = "Mayfair", LastName = "39",
                    Email = "mayfair@gmail.com", Username = "Mayfair39",
                    Role = "Organizer", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png",
                    ProfilePhotoPath = "/uploads/profile-photos/09c93e0d-49ab-4ca6-91c3-c8d9858c603f.jpg"
                };
                u.PasswordHash = hasher.HashPassword(u, "Mayfair1");
                context.Users.Add(u);
            }

            // Pending organizer - visible in admin panel for approval/rejection demo
            if (!await context.Users.AnyAsync(u => u.Username == "TestOrganizer"))
            {
                var u = new User
                {
                    FirstName = "Test", LastName = "Organizer",
                    Email = "testorganizer@test.com", Username = "TestOrganizer",
                    Role = "Organizer", IsActive = true, IsApproved = false,
                    CreatedAt = DateTime.UtcNow
                };
                u.PasswordHash = hasher.HashPassword(u, "Test1234");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "andreea"))
            {
                var u = new User
                {
                    FirstName = "Andreea", LastName = "Maria",
                    Email = "andreea@test.com", Username = "andreea",
                    Role = "Guest", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Test1234");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "radu"))
            {
                var u = new User
                {
                    FirstName = "Radu", LastName = "Popescu",
                    Email = "radu@test.com", Username = "radu",
                    Role = "Guest", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Test1234");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "bianca"))
            {
                var u = new User
                {
                    FirstName = "Bianca", LastName = "Tudor",
                    Email = "bianca@test.com", Username = "bianca",
                    Role = "Guest", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Test1234");
                context.Users.Add(u);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "StudentBriceag"))
            {
                var u = new User
                {
                    FirstName = "Student", LastName = "Briceag",
                    Email = "studentbriceag@test.com", Username = "StudentBriceag",
                    Role = "Guest", IsActive = true, IsApproved = true,
                    CreatedAt = DateTime.UtcNow, QrCodePath = "/images/QrCode.png"
                };
                u.PasswordHash = hasher.HashPassword(u, "Student1");
                context.Users.Add(u);
            }

            await context.SaveChangesAsync();

            // Profile photos for seeded organizers 
            var eventraStudios = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");
            if (eventraStudios != null && string.IsNullOrEmpty(eventraStudios.ProfilePhotoPath))
                eventraStudios.ProfilePhotoPath = "/images/eventrastudios.png";

            var theLobby = await context.Users.FirstOrDefaultAsync(u => u.Username == "TheLobbyRestaurant");
            if (theLobby != null &&
                (string.IsNullOrEmpty(theLobby.ProfilePhotoPath) || theLobby.ProfilePhotoPath == "/images/thelobby.png"))
                theLobby.ProfilePhotoPath = "/images/thelobby.png";

            var mayfair = await context.Users.FirstOrDefaultAsync(u => u.Username == "Mayfair39");
            if (mayfair != null && string.IsNullOrEmpty(mayfair.ProfilePhotoPath))
                mayfair.ProfilePhotoPath = "/images/mayfair39.png";

            await context.SaveChangesAsync();
        }

        // PENDING ORGANIZER REQUEST 
        private static async Task SeedPendingOrganizerRequest(ApplicationDbContext context)
        {
            var testOrg = await context.Users.FirstOrDefaultAsync(u => u.Username == "TestOrganizer");
            if (testOrg == null) return;

            if (await context.OrganizerApprovalRequests.AnyAsync(r => r.UserId == testOrg.Id))
                return;

            context.OrganizerApprovalRequests.Add(new OrganizerApprovalRequest
            {
                UserId = testOrg.Id,
                BusinessName = "Test Organizer",
                BusinessEmail = "testorganizer@test.com",
                BusinessPhone = "0700000000",
                Description = "This is a test organizer account used for demonstrating the admin approval and rejection flow.",
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        // CATEGORIES 

        private static async Task SeedCategories(ApplicationDbContext context)
        {
            var categories = new List<Category>
            {
                new() { Name = "Conference",  Description = "Professional conferences, business talks, and formal speaking events." },
                new() { Name = "Food & Drinks", Description = "Dining experiences, tastings, brunches, dinners, and drink-related events." },
                new() { Name = "Technology",  Description = "Tech events, innovation meetups, AI sessions, and digital industry gatherings." },
                new() { Name = "Exhibition",  Description = "Art exhibitions, gallery openings, and curated cultural showcases." },
                new() { Name = "Market",      Description = "Local markets, fashion markets, flower fairs, and shopping-style community events." },
                new() { Name = "Workshop",    Description = "Hands-on workshops, creative sessions, and educational practical activities." },
                new() { Name = "Festival",    Description = "Seasonal festivals, outdoor celebrations, and large-scale themed public events." },
                new() { Name = "Networking",  Description = "Business networking sessions, professional mixers, and connection-building events." },
                new() { Name = "Social",      Description = "Casual social events, parties, openings, and community gatherings." }
            };

            foreach (var cat in categories)
            {
                if (!await context.Categories.AnyAsync(c => c.Name == cat.Name))
                    context.Categories.Add(cat);
            }

            await context.SaveChangesAsync();
        }

        // EVENTS

        private static async Task SeedEvents(ApplicationDbContext context)
        {
            var eventraStudios = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");
            var theLobby      = await context.Users.FirstOrDefaultAsync(u => u.Username == "TheLobbyRestaurant");
            var mayfair39     = await context.Users.FirstOrDefaultAsync(u => u.Username == "Mayfair39");
            if (eventraStudios == null || theLobby == null || mayfair39 == null) return;

            var conference  = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Conference");
            var food        = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Food & Drinks");
            var tech        = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            var exhibition  = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Exhibition");
            var market      = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Market");
            var workshop    = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Workshop");
            var festival    = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Festival");
            var networking  = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Networking");
            var social      = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Social");

            var events = new List<Event>
            {
                // EVENTRA STUDIOS
                new()
                {
                    Title = "Garden Party",
                    Description = "Garden Party is a refined outdoor social gathering hosted by Guava in Bucharest. Guests are invited to enjoy a relaxed summer atmosphere, elegant decor, light refreshments, and a stylish casual dress code. It is the perfect event for socializing, taking beautiful photos, and spending a warm afternoon in a charming garden setting.",
                    EventDate = new DateTime(2026, 7, 8), StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(18, 0, 0),
                    City = "Bucharest", Location = "Guava", AddressLine = "Str. Tarmului 9",
                    Capacity = 70, AvailableSeats = 28, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/GardenParty.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = social?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Boost Your Business AI Event",
                    Description = "Boost Your Business AI Event is a forward-looking gathering for companies and professionals interested in artificial intelligence and its impact on the business world. The event introduces practical ways AI can improve workflows, innovation, decision-making, and business performance. Attendees will discover insights, examples, and inspiration for entering the new AI era with confidence.",
                    EventDate = new DateTime(2026, 10, 5), StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Craiova", Location = "Universitatea din Craiova", AddressLine = "Universitatea din Craiova",
                    Capacity = 120, AvailableSeats = 42, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/AIEvent.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = tech?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Art Exhibition",
                    Description = "The Art Exhibition brings together creativity, color, and expression in a curated artistic setting in Bucharest. Visitors can explore a collection of works, attend live painting sessions, discover self-portrait art, and enjoy a vibrant creative atmosphere. This event is ideal for art enthusiasts, students, creators, and anyone looking for an inspiring cultural experience.",
                    EventDate = new DateTime(2030, 5, 1), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(20, 0, 0),
                    City = "Bucharest", Location = "Calea Mosilor", AddressLine = "Calea Mosilor, Bucharest",
                    Capacity = 150, AvailableSeats = 58, Price = 35, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/Art Exhibition Event Poster.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = exhibition?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Flower Market",
                    Description = "Flower Market is a dreamy and colorful experience dedicated to flowers, aesthetics, and local creativity. Visitors can browse floral arrangements, home decor, handmade items, and seasonal inspiration in a soft, elegant atmosphere. It is a perfect daytime event for people who enjoy beauty, design, and relaxed weekend outings.",
                    EventDate = new DateTime(2026, 3, 18), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Craiova", Location = "Calea Bucuresti", AddressLine = "Calea Bucuresti, Craiova",
                    Capacity = 500, AvailableSeats = 500, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/Flower Market Event Poster.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = market?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Bloom Cafe Fashion Market",
                    Description = "Bloom Cafe Fashion Market is a chic community event where participants can shop, sell, and discover stylish clothing pieces in a lively and colorful setting. Hosted in Craiova, the event brings together fashion lovers, local sellers, and visitors looking for unique wardrobe finds, inspiration, and a fun social shopping experience.",
                    EventDate = new DateTime(2026, 6, 15), StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(16, 0, 0),
                    City = "Craiova", Location = "Bloom Cafe", AddressLine = "Bloom Cafe, Craiova",
                    Capacity = 90, AvailableSeats = 31, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/Bloom Cafe Event.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = market?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Business Conference",
                    Description = "Join us for the Business Conference in Craiova, a professional gathering designed to bring together entrepreneurs, professionals and innovators. The event focuses on exchanging ideas, sharing experiences, and discussing strategies for growth in today's business environment. It is an excellent opportunity to network, learn from others, and build valuable business connections.",
                    EventDate = new DateTime(2026, 6, 22), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0),
                    City = "Craiova", Location = "Hilton Garden Inn", AddressLine = "Hilton Garden Inn, Craiova",
                    Capacity = 80, AvailableSeats = 27, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/BusinessConferene Event.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = conference?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Pumpkin Carving Contest",
                    Description = "Pumpkin Carving Contest is a playful autumn event for people who love Halloween vibes, seasonal decorations, and creative activities. Guests can take part in the carving contest, enjoy drinks, snap photos at the photobooth, and spend a festive day in a fun outdoor atmosphere. It is perfect for friends, families, and anyone who enjoys themed seasonal events.",
                    EventDate = new DateTime(2026, 10, 30), StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0),
                    City = "Craiova", Location = "Piața Mihai Viteazul", AddressLine = "Piața Mihai Viteazul, Craiova",
                    Capacity = 100, AvailableSeats = 44, Price = 10, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/PumpkinCarving.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = workshop?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Fun Craft Workshop",
                    Description = "Fun Craft Workshop is a cheerful and creative event designed especially for children. The workshop includes all materials needed for a hands-on crafting session where kids can explore colors, shapes, and artistic ideas in a playful environment. It is a great activity for families looking for an educational and entertaining afternoon in Bucharest.",
                    EventDate = new DateTime(2026, 5, 14), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0),
                    City = "Bucharest", Location = "Herastrau Park", AddressLine = "Herastrau Park, Bucharest",
                    Capacity = 40, AvailableSeats = 17, Price = 40, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/workshop.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = workshop?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Opening of Borcelle Coffee",
                    Description = "Celebrate the official opening of Borcelle Coffee with a cozy and stylish event dedicated to coffee lovers. Guests will enjoy a welcoming atmosphere, signature drinks, and a first look at the new location. This launch event is perfect for discovering the brand, meeting other guests, and enjoying a warm social experience in the heart of Bucharest.",
                    EventDate = new DateTime(2026, 9, 8), StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Bucharest", Location = "Borcelle Coffee", AddressLine = "Calea Victoriei, Bucharest",
                    Capacity = 90, AvailableSeats = 34, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/Opening Event.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = social?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Spring Festival",
                    Description = "Spring Festival is a light, fresh, and joyful event celebrating the beauty of the season through flowers, community, and an elegant open-air atmosphere. Hosted in Cluj-Napoca, the festival invites visitors to enjoy spring-themed decor, a welcoming crowd, and a colorful celebration in one of the city's vibrant public spaces.",
                    EventDate = new DateTime(2026, 5, 29), StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Cluj-Napoca", Location = "Piața Unirii", AddressLine = "Piața Unirii, Cluj-Napoca",
                    Capacity = 200, AvailableSeats = 86, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/Pink And Blue Floral Spring Festival Flyer.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = festival?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Business Networking Event",
                    Description = "Business Networking Event is designed for professionals who want to expand their network and discover new opportunities. The event includes networking sessions, an interactive workshop, and a talk show with panel discussion. It is an ideal setting for entrepreneurs, young professionals, and anyone interested in building strong business connections in Craiova.",
                    EventDate = new DateTime(2026, 7, 5), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(12, 30, 0),
                    City = "Craiova", Location = "Calea Bucuresti", AddressLine = "Calea Bucuresti, Craiova",
                    Capacity = 100, AvailableSeats = 36, Price = 20, Currency = "USD", IsFreeEntry = false,
                    ImagePath = "/images/NetworkingEvent.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = networking?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Photography Walk",
                    Description = "Photography Walk is a creative outdoor session for photography enthusiasts of all skill levels. Participants explore a curated route through Bucharest with a professional photographer, learning composition tips, capturing street scenes, and discovering the city through a new lens. All camera types welcome, from smartphones to DSLRs.",
                    EventDate = new DateTime(2026, 4, 12), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(13, 0, 0),
                    City = "Bucharest", Location = "Old Town", AddressLine = "Lipscani, Bucharest",
                    Capacity = 30, AvailableSeats = 0, Price = 25, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/PhotographyWalk.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = workshop?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "April Startup Night",
                    Description = "April Startup Night is a lively evening event bringing together founders, investors, and aspiring entrepreneurs in Craiova. The event features lightning pitches from early-stage startups, a panel Q&A, and an open networking session. Whether you are building a company or simply curious about the startup world, this is the place to connect and get inspired.",
                    EventDate = new DateTime(2026, 5, 3), StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Craiova", Location = "Hilton Garden Inn", AddressLine = "Hilton Garden Inn, Craiova",
                    Capacity = 80, AvailableSeats = 5, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/AprilStartupNight.png", OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = networking?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },

                // TheLobbyRestaurant 
                new()
                {
                    Title = "Dinner Party",
                    Description = "Dinner Party is an elegant evening hosted by The Lobby Restaurant in Bucharest. Guests will enjoy a sophisticated dining atmosphere, curated dishes and a refined celebration setting perfect for couples, friends, or special occasions. The event is designed for those who appreciate tasteful decor, quality dining, and a memorable social experience.",
                    EventDate = new DateTime(2026, 9, 19), StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(22, 0, 0),
                    City = "Bucharest", Location = "The Lobby Restaurant", AddressLine = "Calea Victoriei 68, Bucharest",
                    Capacity = 75, AvailableSeats = 19, Price = 120, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/DinnerParty.png", OrganizerDisplayName = "The Lobby Restaurant",
                    SupportEmail = "thelobby@gmail.com", SupportPhone = "0723456789",
                    CategoryId = food?.Id ?? 0, OrganizerId = theLobby.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Italian Dinner",
                    Description = "Italian Dinner is a cozy and elegant themed dining event hosted by The Lobby Restaurant. Guests are invited to enjoy an evening inspired by Italian cuisine, warm ambiance, and a carefully prepared dinner experience. It is ideal for food lovers looking for a charming night out in Bucharest.",
                    EventDate = new DateTime(2026, 6, 17), StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(22, 0, 0),
                    City = "Bucharest", Location = "Tudy's", AddressLine = "Tudor Arghezi 21, Bucharest",
                    Capacity = 50, AvailableSeats = 12, Price = 95, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/ItalianDinner.png", OrganizerDisplayName = "The Lobby Restaurant",
                    SupportEmail = "thelobby@gmail.com", SupportPhone = "0723456789",
                    CategoryId = food?.Id ?? 0, OrganizerId = theLobby.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "Wine and Cheese Night",
                    Description = "Wine and Cheese Night is an elegant late-evening experience designed for guests who enjoy refined tastes and relaxed social atmospheres. Hosted by The Lobby in Bucharest, the event includes premium wine tasting, curated cheese platters, dinner, and live music. It is the perfect setting for a sophisticated night out with friends or a memorable date night in the city.",
                    EventDate = new DateTime(2026, 7, 17), StartTime = new TimeSpan(23, 0, 0), EndTime = new TimeSpan(23, 59, 0),
                    City = "Bucharest", Location = "The Lobby", AddressLine = "Calea Victoriei, Bucharest",
                    Capacity = 60, AvailableSeats = 0, Price = 150, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/Wine tasting Event.png", OrganizerDisplayName = "The Lobby Restaurant",
                    SupportEmail = "thelobby@gmail.com", SupportPhone = "0723456789",
                    CategoryId = food?.Id ?? 0, OrganizerId = theLobby.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },

                // Mayfair39 — approved 
                new()
                {
                    Title = "Spring Brunch",
                    Description = "Spring Brunch is a relaxed yet elegant daytime event hosted by Mayfair 39. Guests can enjoy a carefully curated brunch menu featuring the famous Japanese pancakes, matcha, seasonal cocktails, spring flowers, and soft background music. With its warm social atmosphere, it is the perfect way to spend a Sunday morning with friends in the heart of Bucharest.",
                    EventDate = new DateTime(2026, 4, 19), StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(14, 0, 0),
                    City = "Bucharest", Location = "Mayfair 39", AddressLine = "Calea Victoriei 39, Bucharest",
                    Capacity = 60, AvailableSeats = 0, Price = 85, Currency = "RON", IsFreeEntry = false,
                    ImagePath = "/images/SpringBrunch.png", OrganizerDisplayName = "Mayfair 39",
                    SupportEmail = "mayfair@gmail.com", SupportPhone = "0701234567",
                    CategoryId = food?.Id ?? 0, OrganizerId = mayfair39.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },

                new()
                {
                    Title = "Summer Brunch Party",
                    Description = "Join us for a stylish summer brunch party at Mayfair 39, in the heart of Bucharest. Enjoy fluffy, famous pancakes served with delicious toppings, alongside creamy lattes and refreshing iced matcha lattes. Expect a cozy summer atmosphere with elegant décor, good music, cute drinks, and a relaxed brunch setting perfect for friends, photos, and sunny weekend vibes. Entrance is free and guests only pay for what they order. A minimum spend of 150 RON per table applies.",
                    EventDate = new DateTime(2026, 7, 17), StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(16, 0, 0),
                    City = "Bucharest", Location = "Mayfair 39", AddressLine = "Calea Victoriei 39, Bucharest",
                    Capacity = 60, AvailableSeats = 60, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/SummerBrunchPartyMayfair.png", OrganizerDisplayName = "Mayfair 39",
                    SupportEmail = "mayfair@gmail.com", SupportPhone = "0701234567",
                    CategoryId = food?.Id ?? 0, OrganizerId = mayfair39.Id,
                    Status = "Approved", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ApprovedAt = DateTime.UtcNow
                },

                // Pending event - admin approval demo 
                new()
                {
                    Title = "Test Event",
                    Description = "This is a test event used for demonstrating the admin event approval, rejection flow and it is seeded through the DbSeeder. Create your accounts to fully test the application.",
                    EventDate = new DateTime(2026, 8, 1), StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(21, 0, 0),
                    City = "Bucharest", Location = "Test Location", AddressLine = "Test Address, Bucharest",
                    Capacity = 50, AvailableSeats = 50, Price = 0, Currency = "RON", IsFreeEntry = true,
                    ImagePath = "/images/Eventraprf.png",
                    OrganizerDisplayName = "Eventra Studios",
                    SupportEmail = "eventra@gmail.com", SupportPhone = "0712345678",
                    CategoryId = social?.Id ?? 0, OrganizerId = eventraStudios.Id,
                    Status = "PendingApproval", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var ev in events)
            {
                if (!await context.Events.AnyAsync(e => e.Title == ev.Title))
                    context.Events.Add(ev);
            }

            await context.SaveChangesAsync();
        }

        // REGISTRATIONS 

        private static async Task SeedParticipations(ApplicationDbContext context)
        {
            var briceag  = await context.Users.FirstOrDefaultAsync(u => u.Username == "StudentBriceag");
            var andreea  = await context.Users.FirstOrDefaultAsync(u => u.Username == "andreea");
            var radu     = await context.Users.FirstOrDefaultAsync(u => u.Username == "radu");
            var bianca   = await context.Users.FirstOrDefaultAsync(u => u.Username == "bianca");

            // StudentBriceag - past events attended
            var briceagPastTitles = new[]
            {
                "Flower Market", "Fun Craft Workshop", "Photography Walk",
                "Spring Brunch", "April Startup Night"
            };

            if (briceag != null)
            {
                foreach (var title in briceagPastTitles)
                {
                    var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                    if (ev == null) continue;
                    if (await context.EventRegistrations.AnyAsync(r => r.UserId == briceag.Id && r.EventId == ev.Id))
                        continue;
                    context.EventRegistrations.Add(new EventRegistration
                    {
                        UserId = briceag.Id, EventId = ev.Id,
                        RegisteredAt = ev.EventDate.AddDays(-7),
                        Status = "Registered", QrToken = Guid.NewGuid().ToString()
                    });
                }

                // StudentBriceag - future events registered
                var briceagFutureTitles = new[] { "Garden Party", "Business Conference" };
                foreach (var title in briceagFutureTitles)
                {
                    var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                    if (ev == null) continue;
                    if (await context.EventRegistrations.AnyAsync(r => r.UserId == briceag.Id && r.EventId == ev.Id))
                        continue;
                    context.EventRegistrations.Add(new EventRegistration
                    {
                        UserId = briceag.Id, EventId = ev.Id,
                        RegisteredAt = DateTime.UtcNow.AddDays(-3),
                        Status = "Registered", QrToken = Guid.NewGuid().ToString()
                    });
                }
            }

            // andreea - future event registered
            if (andreea != null)
            {
                var gardenParty = await context.Events.FirstOrDefaultAsync(e => e.Title == "Garden Party");
                if (gardenParty != null &&
                    !await context.EventRegistrations.AnyAsync(r => r.UserId == andreea.Id && r.EventId == gardenParty.Id))
                {
                    context.EventRegistrations.Add(new EventRegistration
                    {
                        UserId = andreea.Id, EventId = gardenParty.Id,
                        RegisteredAt = DateTime.UtcNow.AddDays(-5),
                        Status = "Registered", QrToken = Guid.NewGuid().ToString()
                    });
                }
            }

            // radu - future event registered
            if (radu != null)
            {
                var springFestival = await context.Events.FirstOrDefaultAsync(e => e.Title == "Spring Festival");
                if (springFestival != null &&
                    !await context.EventRegistrations.AnyAsync(r => r.UserId == radu.Id && r.EventId == springFestival.Id))
                {
                    context.EventRegistrations.Add(new EventRegistration
                    {
                        UserId = radu.Id, EventId = springFestival.Id,
                        RegisteredAt = DateTime.UtcNow.AddDays(-4),
                        Status = "Registered", QrToken = Guid.NewGuid().ToString()
                    });
                }
            }

            // bianca - future event registered
            if (bianca != null)
            {
                var bloomCafe = await context.Events.FirstOrDefaultAsync(e => e.Title == "Bloom Cafe Fashion Market");
                if (bloomCafe != null &&
                    !await context.EventRegistrations.AnyAsync(r => r.UserId == bianca.Id && r.EventId == bloomCafe.Id))
                {
                    context.EventRegistrations.Add(new EventRegistration
                    {
                        UserId = bianca.Id, EventId = bloomCafe.Id,
                        RegisteredAt = DateTime.UtcNow.AddDays(-2),
                        Status = "Registered", QrToken = Guid.NewGuid().ToString()
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // WAITING LIST 

        private static async Task SeedWaitingList(ApplicationDbContext context)
        {
            var briceag = await context.Users.FirstOrDefaultAsync(u => u.Username == "StudentBriceag");
            var andreea = await context.Users.FirstOrDefaultAsync(u => u.Username == "andreea");
            var radu    = await context.Users.FirstOrDefaultAsync(u => u.Username == "radu");
            var bianca  = await context.Users.FirstOrDefaultAsync(u => u.Username == "bianca");

            // Wine and Cheese Night is full - StudentBriceag #1, andreea #2
            var wineNight = await context.Events.FirstOrDefaultAsync(e => e.Title == "Wine and Cheese Night");
            if (wineNight != null)
            {
                if (briceag != null && !await context.WaitingListEntries.AnyAsync(w => w.UserId == briceag.Id && w.EventId == wineNight.Id))
                    context.WaitingListEntries.Add(new WaitingListEntry
                        { UserId = briceag.Id, EventId = wineNight.Id, Position = 1, JoinedAt = DateTime.UtcNow.AddDays(-2), Status = "Waiting" });

                if (andreea != null && !await context.WaitingListEntries.AnyAsync(w => w.UserId == andreea.Id && w.EventId == wineNight.Id))
                    context.WaitingListEntries.Add(new WaitingListEntry
                        { UserId = andreea.Id, EventId = wineNight.Id, Position = 2, JoinedAt = DateTime.UtcNow.AddDays(-1), Status = "Waiting" });
            }

            await context.SaveChangesAsync();
        }

        // REVIEWS

        private static async Task SeedReviews(ApplicationDbContext context)
        {
            var andreea       = await context.Users.FirstOrDefaultAsync(u => u.Username == "andreea");
            var radu          = await context.Users.FirstOrDefaultAsync(u => u.Username == "radu");
            var bianca        = await context.Users.FirstOrDefaultAsync(u => u.Username == "bianca");
            var briceag       = await context.Users.FirstOrDefaultAsync(u => u.Username == "StudentBriceag");
            var eventraStudios = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");
            var theLobby      = await context.Users.FirstOrDefaultAsync(u => u.Username == "TheLobbyRestaurant");
            var mayfair39     = await context.Users.FirstOrDefaultAsync(u => u.Username == "Mayfair39");

            if (eventraStudios == null || theLobby == null || mayfair39 == null) return;

            // Approved reviews for EventraStudios 
            if (andreea != null && !await context.Reviews.AnyAsync(r => r.UserId == andreea.Id && r.OrganizerId == eventraStudios.Id && r.EventId == null))
                context.Reviews.Add(new Review
                {
                    UserId = andreea.Id, OrganizerId = eventraStudios.Id,
                    Rating = 5,
                    Comment = "The platform is super easy to use and the event pages look so elegant. I found two great workshops in less than five minutes.",
                    IsApproved = true, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-10)
                });

            if (radu != null && !await context.Reviews.AnyAsync(r => r.UserId == radu.Id && r.OrganizerId == eventraStudios.Id && r.EventId == null))
                context.Reviews.Add(new Review
                {
                    UserId = radu.Id, OrganizerId = eventraStudios.Id,
                    Rating = 5,
                    Comment = "I love how clearly everything is displayed: location, date, price and available spots. It feels premium and organized.",
                    IsApproved = true, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-8)
                });

            if (bianca != null && !await context.Reviews.AnyAsync(r => r.UserId == bianca.Id && r.OrganizerId == eventraStudios.Id && r.EventId == null))
                context.Reviews.Add(new Review
                {
                    UserId = bianca.Id, OrganizerId = eventraStudios.Id,
                    Rating = 4,
                    Comment = "Perfect for discovering stylish events in Romania. The featured section helped me find events I would have missed otherwise.",
                    IsApproved = true, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-6)
                });

            // Approved reviews for TheLobbyRestaurant 
            if (andreea != null && !await context.Reviews.AnyAsync(r => r.UserId == andreea.Id && r.OrganizerId == theLobby.Id && r.EventId == null))
                context.Reviews.Add(new Review
                {
                    UserId = andreea.Id, OrganizerId = theLobby.Id,
                    Rating = 5,
                    Comment = "An unforgettable evening at The Lobby. The Dinner Party exceeded every expectation — from the elegant decor to the impeccably curated menu. We will definitely be back.",
                    IsApproved = true, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-14)
                });

            // Approved review for Mayfair39 
            if (bianca != null && !await context.Reviews.AnyAsync(r => r.UserId == bianca.Id && r.OrganizerId == mayfair39.Id && r.EventId == null))
                context.Reviews.Add(new Review
                {
                    UserId = bianca.Id, OrganizerId = mayfair39.Id,
                    Rating = 5,
                    Comment = "Spring Brunch at Mayfair 39 was absolutely magical. The Japanese pancakes were as impressive as they sound and the matcha cocktails were unlike anything I have tried. The space is beautifully curated.",
                    IsApproved = true, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-5)
                });

            // StudentBriceag - pending review (teacher demo: appears in admin Pending queue) 
            var aprilStartupNight = await context.Events.FirstOrDefaultAsync(e => e.Title == "April Startup Night");
            if (briceag != null && eventraStudios != null && aprilStartupNight != null &&
                !await context.Reviews.AnyAsync(r => r.UserId == briceag.Id && r.EventId == aprilStartupNight.Id))
            {
                context.Reviews.Add(new Review
                {
                    UserId = briceag.Id, OrganizerId = eventraStudios.Id, EventId = aprilStartupNight.Id,
                    Rating = 4,
                    Comment = "Really inspiring pitches and a great networking session at the end. The venue at Hilton was well-chosen and the schedule was tight and professional. Would recommend to any student interested in entrepreneurship.",
                    IsApproved = false, IsFlagged = false, CreatedAt = DateTime.UtcNow.AddDays(-1)
                });
            }

            await context.SaveChangesAsync();
        }

        // FAVORITES 

        private static async Task SeedFavorites(ApplicationDbContext context)
        {
            // Set denormalized FavoriteCount on each event 
            var eventCounts = new Dictionary<string, int>
            {
                ["Wine and Cheese Night"]       = 126,
                ["Spring Festival"]             = 56,
                ["Garden Party"]                = 52,
                ["Pumpkin Carving Contest"]     = 47,
                ["Dinner Party"]                = 41,
                ["Business Networking Event"]   = 38,
                ["Opening of Borcelle Coffee"]  = 35,
                ["Boost Your Business AI Event"] = 31,
                ["Bloom Cafe Fashion Market"]   = 29,
                ["Spring Brunch"]               = 139,
                ["Flower Market"]               = 18,
                ["April Startup Night"]         = 17,
                ["Photography Walk"]            = 15,
                ["Italian Dinner"]              = 12,
                ["Fun Craft Workshop"]          = 8,
                ["Art Exhibition"]              = 0,
                ["Business Conference"]         = 0
            };

            foreach (var (title, count) in eventCounts)
            {
                var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                if (ev != null && ev.FavoriteCount == 0)
                    ev.FavoriteCount = count;
            }

            await context.SaveChangesAsync();

            // Seed actual Favorite rows for demo users
            var andreea = await context.Users.FirstOrDefaultAsync(u => u.Username == "andreea");
            var radu    = await context.Users.FirstOrDefaultAsync(u => u.Username == "radu");
            var bianca  = await context.Users.FirstOrDefaultAsync(u => u.Username == "bianca");
            var briceag = await context.Users.FirstOrDefaultAsync(u => u.Username == "StudentBriceag");

            var pairs = new List<(User? user, string title)>
            {
                (andreea, "Wine and Cheese Night"),
                (andreea, "Pumpkin Carving Contest"),
                (andreea, "Bloom Cafe Fashion Market"),
                (radu,    "Spring Festival"),
                (radu,    "Business Networking Event"),
                (radu,    "Italian Dinner"),
                (bianca,  "Garden Party"),
                (bianca,  "Boost Your Business AI Event"),
                (bianca,  "Dinner Party"),
                (briceag, "Bloom Cafe Fashion Market"),
                (briceag, "Business Networking Event")
            };

            foreach (var (user, title) in pairs)
            {
                if (user == null) continue;
                var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                if (ev == null) continue;
                if (await context.Favorites.AnyAsync(f => f.UserId == user.Id && f.EventId == ev.Id)) continue;
                context.Favorites.Add(new Favorite
                    { UserId = user.Id, EventId = ev.Id, CreatedAt = DateTime.UtcNow });
            }

            await context.SaveChangesAsync();
        }

        // NOTIFICATIONS 

        private static async Task SeedNotifications(ApplicationDbContext context)
        {
            var eventraStudios = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");
            var theLobby       = await context.Users.FirstOrDefaultAsync(u => u.Username == "TheLobbyRestaurant");
            var mayfair39      = await context.Users.FirstOrDefaultAsync(u => u.Username == "Mayfair39");
            var briceag        = await context.Users.FirstOrDefaultAsync(u => u.Username == "StudentBriceag");

            // EventraStudios - event approved notifications
            if (eventraStudios != null)
            {
                var eventsForNotif = new[] { "Garden Party", "Spring Festival", "Bloom Cafe Fashion Market", "Business Networking Event", "Art Exhibition" };
                foreach (var title in eventsForNotif)
                {
                    var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                    if (ev == null) continue;
                    if (!await context.Notifications.AnyAsync(n => n.UserId == eventraStudios.Id && n.RelatedEventId == ev.Id && n.Type == "EventApproved"))
                        context.Notifications.Add(new Notification
                        {
                            UserId = eventraStudios.Id, Title = "Event Approved",
                            Message = $"Your event \"{ev.Title}\" was approved by the admin.",
                            Type = "EventApproved", RelatedEventId = ev.Id,
                            IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-14)
                        });
                }
            }

            // TheLobbyRestaurant - event approved notifications
            if (theLobby != null)
            {
                var eventsForNotif = new[] { "Wine and Cheese Night", "Dinner Party", "Italian Dinner" };
                foreach (var title in eventsForNotif)
                {
                    var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                    if (ev == null) continue;
                    if (!await context.Notifications.AnyAsync(n => n.UserId == theLobby.Id && n.RelatedEventId == ev.Id && n.Type == "EventApproved"))
                        context.Notifications.Add(new Notification
                        {
                            UserId = theLobby.Id, Title = "Event Approved",
                            Message = $"Your event \"{ev.Title}\" was approved by the admin.",
                            Type = "EventApproved", RelatedEventId = ev.Id,
                            IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-12)
                        });
                }
            }

            // Mayfair39 - event approved notification 
            if (mayfair39 != null)
            {
                var springBrunch = await context.Events.FirstOrDefaultAsync(e => e.Title == "Spring Brunch");
                if (springBrunch != null && !await context.Notifications.AnyAsync(n => n.UserId == mayfair39.Id && n.RelatedEventId == springBrunch.Id && n.Type == "EventApproved"))
                    context.Notifications.Add(new Notification
                    {
                        UserId = mayfair39.Id, Title = "Event Approved",
                        Message = $"Your event \"{springBrunch.Title}\" was approved by the admin.",
                        Type = "EventApproved", RelatedEventId = springBrunch.Id,
                        IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-10)
                    });
            }

            // StudentBriceag - new event from organizer 
            if (briceag != null)
            {
                var gardenParty = await context.Events.FirstOrDefaultAsync(e => e.Title == "Garden Party");
                if (gardenParty != null && !await context.Notifications.AnyAsync(n => n.UserId == briceag.Id && n.RelatedEventId == gardenParty.Id && n.Type == "NewEventFromOrganizer"))
                    context.Notifications.Add(new Notification
                    {
                        UserId = briceag.Id, Title = "New Event Published",
                        Message = "Eventra Studios published a new event: \"Garden Party\".",
                        Type = "NewEventFromOrganizer", RelatedEventId = gardenParty.Id,
                        IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-7)
                    });

                // StudentBriceag - review reminders for unreviewed past events 
                var reminderTitles = new[] { "Flower Market", "Fun Craft Workshop", "Photography Walk", "Spring Brunch" };
                foreach (var title in reminderTitles)
                {
                    var ev = await context.Events.FirstOrDefaultAsync(e => e.Title == title);
                    if (ev == null) continue;
                    if (!await context.Notifications.AnyAsync(n => n.UserId == briceag.Id && n.RelatedEventId == ev.Id && n.Type == "ReviewReminder"))
                        context.Notifications.Add(new Notification
                        {
                            UserId = briceag.Id, Title = "Leave a Review",
                            Message = $"You attended \"{ev.Title}\". Share your experience by leaving a review!",
                            Type = "ReviewReminder", RelatedEventId = ev.Id,
                            IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-3)
                        });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
