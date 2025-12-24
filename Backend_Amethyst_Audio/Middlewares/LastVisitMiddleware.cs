using System.Security.Claims;
using Backend_Amethyst_Audio.Data;

namespace Backend_Amethyst_Audio.Middlewares;

public class LastVisitMiddleware
{
    private readonly RequestDelegate _next;
    // Настройка интервала: обновляем не чаще чем раз в 15 минут
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(15);

    public LastVisitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // 1. Проверяем, авторизован ли пользователь
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                // 2. Ищем пользователя в БД
                var user = await dbContext.Users.FindAsync(int.Parse(userId));
                
                if (user != null)
                {
                    // 3. Проверяем, нужно ли обновлять дату
                    var now = DateTime.UtcNow;
                    if ((now - user.LastVisit) > UpdateInterval)
                    {
                        user.LastVisit = now;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        // Передаем запрос дальше по конвейеру
        await _next(context);
    }
}