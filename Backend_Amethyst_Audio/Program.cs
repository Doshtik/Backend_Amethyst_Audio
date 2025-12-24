using Backend_Amethyst_Audio.Data;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Middlewares;
using Backend_Amethyst_Audio.Profiles;
using Backend_Amethyst_Audio.Services.Abstractions;
using Backend_Amethyst_Audio.Services.Impementations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetConnectionString("AutoMapperKey");
}, typeof(UserMappingProfile));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LastVisitMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();