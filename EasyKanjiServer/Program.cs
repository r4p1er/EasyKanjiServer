using EasyKanjiServer.Middlewares;
using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connection));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AuthOptions:ISSUER"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AuthOptions:AUDIENCE"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthOptions:KEY"]!))
    };
});

var app = builder.Build();

app.UseMiddleware<JWTCheckMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();