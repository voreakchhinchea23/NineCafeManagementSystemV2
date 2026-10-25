
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NineCafeApi.Authorization;
using NineCafeApi.Authorization;
using NineCafeApi.Data;
using NineCafeApi.Services.Auth;
using NineCafeApi.Services.Modules;
using NineCafeApi.Services.Permissions;
using NineCafeApi.Services.Roles;
using NineCafeApi.Services.Users;
using Scalar.AspNetCore;
using System.Text;

namespace NineCafeApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 1. Add Controllers
            builder.Services.AddControllers();
            // 2. Register DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // 3. Register Custom Authorization Services
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            // 4. In-Memory Cache for Token Blacklist (Logout)
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
            // 5. Authentication & JWT Bearer Configuration
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is missing from appsettings.json.");
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
                // Reject request if token has been blacklisted / logged out
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var blacklistService = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenBlacklistService>();
                        var token = context.Request.Headers["Authorization"]
                            .ToString().Replace("Bearer ", "").Trim();
                        if (blacklistService.IsBlacklisted(token))
                        {
                            context.Fail("This token has been revoked / logged out.");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();
            // 6. Register Application Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IModuleService, ModuleService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            // 7. OpenAPI & Scalar Documentation
            builder.Services.AddOpenApi();
            var app = builder.Build();
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Seed Database on startup
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await DatabaseSeeder.SeedAsync(context);
            }

            app.Run();
        }
    }
}
