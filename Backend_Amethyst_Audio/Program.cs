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
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzk4MDcwNDAwIiwiaWF0IjoiMTc2NjU4ODMyOCIsImFjY291bnRfaWQiOiIwMTliNTBkZTViNzU3NTNmOTkwYjhhYzA1M2I0Mzc0YiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2Q4ZHhhNmtxcGZiZDhuY256ZWRhcDVuIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.uG7QfSqiuXHzshHdqlGb4oK0giL0u_7QWdyTRgC_PbmdI5_JwDinZX75jvGkWuNz2IDvFH4TaIuVDUGhqC_8qXmkSlb-qZ9WbLB0PDMS2kfCYX4FsJCsOBHVG3dG0iRyWnleJ4Vfewoz2iXcJnTnL43hdCRjTat8sOjzTE2EvoVkj2TjvBZOQ-MbR6oPHfqe-O_hB78w76HJsppWG7JMole2YCUTTrChSxekivmfcdLXnlRcuvguPaFBHzFaPsWshGkUhm5aKHwlQznfAmv-dvq00jLWneLKU5V1U1gLMQPkPIpe53mcXL_kHuR5y5B9_jjD9sJdXumKrzCwybw90Q";
}, typeof(MappingProfile));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LastVisitMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();