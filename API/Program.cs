using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Services.Impl;
using API.Services;
using API.Models;
using API.Mapping;
using Quartz;
using API.CronJobs;

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
            builder.Services.AddScoped<IWardService, WardService>();
            builder.Services.AddScoped<IDistrictService, DistrictService>();
            builder.Services.AddScoped<IProvinceService, ProvinceService>();
            builder.Services.AddScoped<ISalaryService, SalaryService>();
            builder.Services.AddScoped<IRuleService, RuleService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IInvoiceService, InvoicesService>();
            builder.Services.AddScoped<CustomAddressResolver>();

            builder.Services.AddAutoMapper(typeof(InvoiceMapper));
            builder.Services.AddAutoMapper(typeof(UserMapper));
            builder.Services.AddAutoMapper(typeof(NewMapper));
            builder.Services.AddAutoMapper(typeof(ProductMapper));
            builder.Services.AddAutoMapper(typeof(PromotionMapper));
            builder.Services.AddAutoMapper(typeof(CardMapper));
            builder.Services.AddAutoMapper(typeof(EmployeeMapper));
            builder.Services.AddAutoMapper(typeof(BranchMapper));
            builder.Services.AddAutoMapper(typeof(SalaryMapper));
            builder.Services.AddAutoMapper(typeof(RuleMapper));
            builder.Services.AddAutoMapper(typeof(RoleMapper));
            builder.Services.AddAutoMapper(typeof(WorkScheduleMapper));
            builder.Services.AddAutoMapper(typeof(ComboMapper));

            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.ScheduleJob<UpdateWorkScheduleJob>(trigger =>
                    trigger.WithIdentity("updateWorkScheduleJob")
                           .StartNow()
                           .WithCronSchedule("0 * * * * ?"));
            });

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

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5129")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
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
            app.UseCors("AllowSpecificOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}
