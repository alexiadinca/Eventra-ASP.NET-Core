using Eventra.Repositories;
using Eventra.Repositories.Interfaces;
using Eventra.Services;
using Eventra.Services.Interfaces;


namespace Eventra.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
            services.AddScoped<IWaitingListRepository, WaitingListRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IEventCheckInRepository, EventCheckInRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }

        public static IServiceCollection AddAppSession(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            return services;
        }
    }
}
