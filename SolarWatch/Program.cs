using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarWatch.Data;
using SolarWatch.Service;
using SolarWatch.Service.Authentication;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        AddServices();
        ConfigureSwagger();
        AddDbContext(configuration);
        AddAuthentication(configuration);
        AddIdentity();

        var app = builder.Build();

        AddRoles();
        AddAdmin();
        DBMigration();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

        void DBMigration()
        {
            // migrate any database changes on startup (includes initial db creation)
            using (var scope = app.Services.CreateScope())
            {
                var solarwatchDataContext = scope.ServiceProvider.GetRequiredService<SolarwatchDbContext>();
                solarwatchDataContext.Database.EnsureCreated();

                var usersDataContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
                usersDataContext.Database.EnsureCreated();
            }
        }

        void AddServices()
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
        }

        void ConfigureSwagger()
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }

                    }
                });
            });
        }

        void AddDbContext(IConfiguration configuration)
        {
            builder.Services.AddDbContext<SolarwatchDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:Default"]);
            });
            builder.Services.AddDbContext<UsersContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:Default"]);
            });
            builder.Services.AddSingleton<IGeocoder, Geocoder>();
            builder.Services.AddSingleton<ITwilightDataFetcher, TwilightDataFetcher>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ITokenService,TokenService>();
            builder.Services.AddScoped<IWeatherDescriptionProvider, WeatherDescriptionProvider>();
        }

        void AddAuthentication(IConfiguration configuration)
        {
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["IssuerSigningKey"])
                        )
                    };
                });
        }

        void AddIdentity()
        {
            builder.Services.AddIdentityCore<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            }).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UsersContext>();
        }

        void AddRoles()
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var tAdmin = CreateAdminRole(roleManager);
            tAdmin.Wait();

            var tUser = CreateUserRole(roleManager);
            tUser.Wait();
        }

        async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        void AddAdmin()
        {
            var tAdmin = CreateAdminIfNotExists();
            tAdmin.Wait();
        }

        async Task CreateAdminIfNotExists()
        {
            using var scope = app.Services.CreateScope();
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var adminInDb = await userManager.FindByEmailAsync("admin@admin.com");
            if (adminInDb == null)
            {
                var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
                var adminCreated = await userManager.CreateAsync(admin, "Admin123");

                if (adminCreated.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}