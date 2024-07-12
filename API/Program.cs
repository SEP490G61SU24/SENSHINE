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
            builder.Services.AddScoped<IComboService, ComboServicee>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
            builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
            builder.Services.AddScoped<IBedService, BedService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IPromotionService, PromotionService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddAutoMapper(typeof(NewMapper));
            builder.Services.AddAutoMapper(typeof(ProductMapper));
            builder.Services.AddAutoMapper(typeof(PromotionMapper));
<<<<<<< HEAD
=======
            builder.Services.AddAutoMapper(typeof(CardMapper));
            builder.Services.AddAutoMapper(typeof(EmployeeMapper));
            builder.Services.AddAutoMapper(typeof(BranchMapper));
>>>>>>> 6092d6b338fa976535aef1de83e4da8d0dc48f26

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
