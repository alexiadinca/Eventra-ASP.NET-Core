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
            await SeedCategories(context);
            await SeedEvents(context);
            await SeedHomeReviews(context);
        }

        private static async Task SeedUsers(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher<User>();

            if (!await context.Users.AnyAsync(u => u.Username == "AlexiaDinca"))
            {
                var user = new User
                {
                    FirstName = "Alexia",
                    LastName = "Dinca",
                    Email = "alexiadinca@gmail.com",
                    Username = "AlexiaDinca",
                    Role = "Guest",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Student1");
                context.Users.Add(user);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "EventraStudios"))
            {
                var user = new User
                {
                    FirstName = "Eventra",
                    LastName = "Studios",
                    Email = "eventra@gmail.com",
                    Username = "EventraStudios",
                    Role = "Organizer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Eventra1");
                context.Users.Add(user);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "TheLobbyRestaurant"))
            {
                var user = new User
                {
                    FirstName = "The Lobby",
                    LastName = "Restaurant",
                    Email = "thelobby@gmail.com",
                    Username = "TheLobbyRestaurant",
                    Role = "Organizer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Thelobby1");
                context.Users.Add(user);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "andreea"))
            {
                var user = new User
                {
                    FirstName = "Andreea",
                    LastName = "Maria",
                    Email = "andreea@test.com",
                    Username = "andreea",
                    Role = "Guest",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Test1234");
                context.Users.Add(user);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "radu"))
            {
                var user = new User
                {
                    FirstName = "Radu",
                    LastName = "Popescu",
                    Email = "radu@test.com",
                    Username = "radu",
                    Role = "Guest",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Test1234");
                context.Users.Add(user);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "bianca"))
            {
                var user = new User
                {
                    FirstName = "Bianca",
                    LastName = "Tudor",
                    Email = "bianca@test.com",
                    Username = "bianca",
                    Role = "Guest",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    QrCodePath = "/images/QrCode.png"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Test1234");
                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }



        private static async Task SeedHomeReviews(ApplicationDbContext context)
        {
            if (await context.Reviews.AnyAsync())
                return;

            var andreea = await context.Users.FirstOrDefaultAsync(u => u.Username == "andreea");
            var radu = await context.Users.FirstOrDefaultAsync(u => u.Username == "radu");
            var bianca = await context.Users.FirstOrDefaultAsync(u => u.Username == "bianca");
            var organizer = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");

            if (andreea == null || radu == null || bianca == null || organizer == null)
                return;

            var reviews = new List<Review>
            {
                new Review
                {
                    UserId = andreea.Id,
                    OrganizerId = organizer.Id,
                    Rating = 5,
                    Comment = "The platform is super easy to use and the event pages look so elegant. I found two great workshops in less than five minutes.",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Review
                {
                    UserId = radu.Id,
                    OrganizerId = organizer.Id,
                    Rating = 5,
                    Comment = "I love how clearly everything is displayed: location, date, price and available spots. It feels premium and organized.",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Review
                {
                    UserId = bianca.Id,
                    OrganizerId = organizer.Id,
                    Rating = 4,
                    Comment = "Perfect for discovering stylish events in Romania. The featured section helped me find events I would have missed otherwise.",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
        private static async Task SeedCategories(ApplicationDbContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Conference",
                    Description = "Professional conferences, business talks, and formal speaking events."
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Dining experiences, tastings, brunches, dinners, and drink-related events."
                },
                new Category
                {
                    Name = "Technology",
                    Description = "Tech events, innovation meetups, AI sessions, and digital industry gatherings."
                },
                new Category
                {
                    Name = "Exhibition",
                    Description = "Art exhibitions, gallery openings, and curated cultural showcases."
                },
                new Category
                {
                    Name = "Market",
                    Description = "Local markets, fashion markets, flower fairs, and shopping-style community events."
                },
                new Category
                {
                    Name = "Workshop",
                    Description = "Hands-on workshops, creative sessions, and educational practical activities."
                },
                new Category
                {
                    Name = "Festival",
                    Description = "Seasonal festivals, outdoor celebrations, and large-scale themed public events."
                },
                new Category
                {
                    Name = "Networking",
                    Description = "Business networking sessions, professional mixers, and connection-building events."
                },
                new Category
                {
                    Name = "Social",
                    Description = "Casual social events, parties, openings, and community gatherings."
                }
            };

            foreach (var category in categories)
            {
                bool exists = await context.Categories.AnyAsync(c => c.Name == category.Name);
                if (!exists)
                {
                    context.Categories.Add(category);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedEvents(ApplicationDbContext context)
        {
            User? eventraStudios = await context.Users.FirstOrDefaultAsync(u => u.Username == "EventraStudios");
            User? theLobby = await context.Users.FirstOrDefaultAsync(u => u.Username == "TheLobbyRestaurant");

            if (eventraStudios == null || theLobby == null)
                return;

            
            Category conferenceCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Conference");
            var foodCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Food & Drinks");
            var techCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            var exhibitionCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Exhibition");
            var marketCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Market");
            var workshopCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Workshop");
            var festivalCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Festival");
            var networkingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Networking");
            var socialCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Social");

            var events = new List<Event>
    {
        new Event
        {
            Title = "Garden Party",
            Description = "Garden Party is a refined outdoor social gathering hosted by Guava in Bucharest. Guests are invited to enjoy a relaxed summer atmosphere, elegant decor, light refreshments, and a stylish casual dress code. It is the perfect event for socializing, taking beautiful photos, and spending a warm afternoon in a charming garden setting.",
            EventDate = new DateTime(2026, 7, 8),
            StartTime = new TimeSpan(15, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            City = "Bucharest",
            Location = "Guava",
            AddressLine = "Str. Tarmului 9",
            Capacity = 70,
            AvailableSeats = 28,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/GardenParty.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = socialCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Boost Your Business AI Event",
            Description = "Boost Your Business AI Event is a forward-looking gathering for companies and professionals interested in artificial intelligence and its impact on the business world. The event introduces practical ways AI can improve workflows, innovation, decision-making, and business performance. Attendees will discover insights, examples, and inspiration for entering the new AI era with confidence.",
            EventDate = new DateTime(2026, 10, 5),
            StartTime = new TimeSpan(19, 0, 0),
            EndTime = new TimeSpan(21, 0, 0),
            City = "Craiova",
            Location = "Universitatea din Craiova",
            AddressLine = "Universitatea din Craiova",
            Capacity = 120,
            AvailableSeats = 42,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/AIEvent.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = techCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Art Exhibition",
            Description = "The Art Exhibition brings together creativity, color, and expression in a curated artistic setting in Bucharest. Visitors can explore a collection of works, attend live painting sessions, discover self-portrait art, and enjoy a vibrant creative atmosphere. This event is ideal for art enthusiasts, students, creators, and anyone looking for an inspiring cultural experience.",
            EventDate = new DateTime(2030, 5, 1),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            City = "Bucharest",
            Location = "Calea Mosilor",
            AddressLine = "Calea Mosilor, Bucharest",
            Capacity = 150,
            AvailableSeats = 58,
            Price = 35,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/Art Exhibition Event Poster.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = exhibitionCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Flower Market",
            Description = "Flower Market is a dreamy and colorful experience dedicated to flowers, aesthetics, and local creativity. Visitors can browse floral arrangements, home decor, handmade items, and seasonal inspiration in a soft, elegant atmosphere. It is a perfect daytime event for people who enjoy beauty, design, and relaxed weekend outings.",
            EventDate = new DateTime(2026, 3, 18),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(21, 0, 0),
            City = "Craiova",
            Location = "Calea Bucuresti",
            AddressLine = "Calea Bucuresti, Craiova",
            Capacity = 500,
            AvailableSeats = 500,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/Flower Market Event Poster.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = marketCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Bloom Cafe Fashion Market",
            Description = "Bloom Cafe Fashion Market is a chic community event where participants can shop, sell, and discover stylish clothing pieces in a lively and colorful setting. Hosted in Craiova, the event brings together fashion lovers, local sellers, and visitors looking for unique wardrobe finds, inspiration, and a fun social shopping experience.",
            EventDate = new DateTime(2026, 6, 15),
            StartTime = new TimeSpan(15, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            City = "Craiova",
            Location = "Bloom Cafe",
            AddressLine = "Bloom Cafe, Craiova",
            Capacity = 90,
            AvailableSeats = 31,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/Bloom Cafe Event.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = marketCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Business Conference",
            Description = "Join us for the Business Conference in Craiova, a professional gathering designed to bring together entrepreneurs, professionals and innovators. The event focuses on exchanging ideas, sharing experiences, and discussing strategies for growth in today’s business environment. It is an excellent opportunity to network, learn from others, and build valuable business connections.",
            EventDate = new DateTime(2026, 6, 22),
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            City = "Craiova",
            Location = "Hilton Garden Inn",
            AddressLine = "Hilton Garden Inn, Craiova",
            Capacity = 80,
            AvailableSeats = 27,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/BusinessConferene Event.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = conferenceCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Dinner Party",
            Description = "Dinner Party is an elegant evening hosted by The Lobby Restaurant in Bucharest. Guests will enjoy a sophisticated dining atmosphere, curated dishes and a refined celebration setting perfect for couples, friends, or special occasions. The event is designed for those who appreciate tasteful decor, quality dining, and a memorable social experience.",
            EventDate = new DateTime(2026, 9, 19),
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(22, 0, 0),
            City = "Bucharest",
            Location = "The Lobby Restaurant",
            AddressLine = "Calea Victoriei 68, Bucharest",
            Capacity = 75,
            AvailableSeats = 19,
            Price = 120,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/DinnerParty.png",
            OrganizerDisplayName = "The Lobby Restaurant",
            SupportEmail = "thelobby@gmail.com",
            SupportPhone = "0723456789",
            CategoryId = foodCategory?.Id ?? 0,
            OrganizerId = theLobby.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Italian Dinner",
            Description = "Italian Dinner is a cozy and elegant themed dining event hosted by The Lobby Restaurant. Guests are invited to enjoy an evening inspired by Italian cuisine, warm ambiance, and a carefully prepared dinner experience. It is ideal for food lovers looking for a charming night out in Bucharest.",
            EventDate = new DateTime(2026, 5, 17),
            StartTime = new TimeSpan(19, 0, 0),
            EndTime = new TimeSpan(22, 0, 0),
            City = "Bucharest",
            Location = "Tudy's",
            AddressLine = "Tudor Arghezi 21, Bucharest",
            Capacity = 50,
            AvailableSeats = 12,
            Price = 95,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/ItalianDinner.png",
            OrganizerDisplayName = "The Lobby Restaurant",
            SupportEmail = "thelobby@gmail.com",
            SupportPhone = "0723456789",
            CategoryId = foodCategory?.Id ?? 0,
            OrganizerId = theLobby.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Pumpkin Carving Contest",
            Description = "Pumpkin Carving Contest is a playful autumn event for people who love Halloween vibes, seasonal decorations, and creative activities. Guests can take part in the carving contest, enjoy drinks, snap photos at the photobooth, and spend a festive day in a fun outdoor atmosphere. It is perfect for friends, families, and anyone who enjoys themed seasonal events.",
            EventDate = new DateTime(2026, 10, 30),
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            City = "Craiova",
            Location = "Piața Mihai Viteazul",
            AddressLine = "Piața Mihai Viteazul, Craiova",
            Capacity = 100,
            AvailableSeats = 44,
            Price = 10,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/PumpkinCarving.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = workshopCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Wine and Cheese Night",
            Description = "Wine and Cheese Night is an elegant late-evening experience designed for guests who enjoy refined tastes and relaxed social atmospheres. Hosted by The Lobby in Bucharest, the event includes premium wine tasting, curated cheese platters, dinner, and live music. It is the perfect setting for a sophisticated night out with friends or a memorable date night in the city.",
            EventDate = new DateTime(2026, 7, 17),
            StartTime = new TimeSpan(23, 0, 0),
            EndTime = new TimeSpan(23, 59, 0),
            City = "Bucharest",
            Location = "The Lobby",
            AddressLine = "Calea Victoriei, Bucharest",
            Capacity = 60,
            AvailableSeats = 0,
            Price = 150,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/Wine tasting Event.png",
            OrganizerDisplayName = "The Lobby Restaurant",
            SupportEmail = "thelobby@gmail.com",
            SupportPhone = "0723456789",
            CategoryId = foodCategory?.Id ?? 0,
            OrganizerId = theLobby.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Fun Craft Workshop",
            Description = "Fun Craft Workshop is a cheerful and creative event designed especially for children. The workshop includes all materials needed for a hands-on crafting session where kids can explore colors, shapes, and artistic ideas in a playful environment. It is a great activity for families looking for an educational and entertaining afternoon in Bucharest.",
            EventDate = new DateTime(2026, 5, 14),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            City = "Bucharest",
            Location = "Herastrau Park",
            AddressLine = "Herastrau Park, Bucharest",
            Capacity = 40,
            AvailableSeats = 17,
            Price = 40,
            Currency = "RON",
            IsFreeEntry = false,
            ImagePath = "/images/workshop.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = workshopCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Opening of Borcelle Coffee",
            Description = "Celebrate the official opening of Borcelle Coffee with a cozy and stylish event dedicated to coffee lovers. Guests will enjoy a welcoming atmosphere, signature drinks, and a first look at the new location. This launch event is perfect for discovering the brand, meeting other guests, and enjoying a warm social experience in the heart of Bucharest.",
            EventDate = new DateTime(2026, 9, 8),
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(21, 0, 0),
            City = "Bucharest",
            Location = "Borcelle Coffee",
            AddressLine = "Calea Victoriei, Bucharest",
            Capacity = 90,
            AvailableSeats = 34,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/Opening Event.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = socialCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Spring Festival",
            Description = "Spring Festival is a light, fresh, and joyful event celebrating the beauty of the season through flowers, community, and an elegant open-air atmosphere. Hosted in Cluj-Napoca, the festival invites visitors to enjoy spring-themed decor, a welcoming crowd, and a colorful celebration in one of the city’s vibrant public spaces.",
            EventDate = new DateTime(2026, 5, 29),
            StartTime = new TimeSpan(17, 0, 0),
            EndTime = new TimeSpan(21, 0, 0),
            City = "Cluj-Napoca",
            Location = "Piața Unirii",
            AddressLine = "Piața Unirii, Cluj-Napoca",
            Capacity = 200,
            AvailableSeats = 86,
            Price = 0,
            Currency = "RON",
            IsFreeEntry = true,
            ImagePath = "/images/Pink And Blue Floral Spring Festival Flyer.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = festivalCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        },

        new Event
        {
            Title = "Business Networking Event",
            Description = "Business Networking Event is designed for professionals who want to expand their network and discover new opportunities. The event includes networking sessions, an interactive workshop, and a talk show with panel discussion. It is an ideal setting for entrepreneurs, young professionals, and anyone interested in building strong business connections in Craiova.",
            EventDate = new DateTime(2026, 7, 5),
            StartTime = new TimeSpan(8, 30, 0),
            EndTime = new TimeSpan(12, 30, 0),
            City = "Craiova",
            Location = "Calea Bucuresti",
            AddressLine = "Calea Bucuresti, Craiova",
            Capacity = 100,
            AvailableSeats = 36,
            Price = 20,
            Currency = "USD",
            IsFreeEntry = false,
            ImagePath = "/images/NetworkingEvent.png",
            OrganizerDisplayName = "Eventra Studios",
            SupportEmail = "eventra@gmail.com",
            SupportPhone = "0712345678",
            CategoryId = networkingCategory?.Id ?? 0,
            OrganizerId = eventraStudios.Id,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow
        }
    };

            foreach (var ev in events)
            {
                bool exists = await context.Events.AnyAsync(e => e.Title == ev.Title);
                if (!exists)
                    context.Events.Add(ev);
            }

            await context.SaveChangesAsync();
        }
    }
}