using Eventra.Data;

namespace Eventra.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            var userId = context.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = await db.Users.FindAsync(userId.Value);
                if (user == null || !user.IsActive)
                    context.Session.Clear();
            }

            await _next(context);
        }
    }
}
