namespace Backend_Amethyst_Audio.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    
}