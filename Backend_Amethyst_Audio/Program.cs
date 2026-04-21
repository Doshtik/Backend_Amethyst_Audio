using System.Text;
using Backend_Amethyst_Audio.Middlewares;
using Backend_Amethyst_Audio.Profiles;
using Backend_Amethyst_Audio.Services.Abstractions;
using Backend_Amethyst_Audio.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Backend_Amethyst_Audio.Models.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"), o => 
        o.MigrationsHistoryTable("__EFMigrationsHistory", "public")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetConnectionString("AutoMapperKey");
}, typeof(UserMappingProfile));
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetConnectionString("AutoMapperKey");
}, typeof(TrackMappingProfile));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITrackService, TracksService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Issuer"]).ToString(), // Кто выпустил
            ValidateAudience = true,
            ValidAudience = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Audience"]).ToString(), // Для кого
            ValidateLifetime = true, // Не просрочен ли
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuerSigningKey = true // Проверка подписи ключом
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<LastVisitMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();