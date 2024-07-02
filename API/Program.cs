using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Services.Impl;
using API.Services;
using API.Models;
using API.Mapping;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            // Configure DbContext
            builder.Services.AddDbContext<SenShineSpaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

            // Register services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISpaService, SpaService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<ICardService, CardService>();

            builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();

            builder.Services.AddScoped<IPromotionService, PromotionService>();
            builder.Services.AddAutoMapper(typeof(NewMapper));

            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Configure authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireCEO", policy => policy.RequireRole("CEO"));
                options.AddPolicy("RequireManager", policy => policy.RequireRole("MANAGER"));
                options.AddPolicy("RequireReceptions", policy => policy.RequireRole("RECEPTIONS"));
                options.AddPolicy("RequireStaff", policy => policy.RequireRole("STAFF"));
            });

            var app = builder.Build();
          
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
