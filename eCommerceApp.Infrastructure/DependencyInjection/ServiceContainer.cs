﻿using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using eCommerceApp.Infrastructure.Middleware;
using eCommerceApp.Infrastructure.Repositories;
using eCommerceApp.Infrastructure.Services;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace eCommerceApp.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services , IConfiguration configuration)
        {
            string connectionString = "Default";
            services.AddDbContext<AppDbContext>(
                options =>
                options.UseSqlServer(configuration.GetConnectionString(connectionString),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure();
                }).UseExceptionProcessor(),
                ServiceLifetime.Scoped);
            services.AddScoped<IGeneric<Product>, GenericRepository<Product>>();
            services.AddScoped<IGeneric<Category>, GenericRepository<Category>>();
            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdapter<>));
            services.AddDefaultIdentity<AppUser>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredUniqueChars = 1;
                }
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                    };
                });
            return services;
        } 

        public static IApplicationBuilder UseInfrastractureService(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExeptionHandlingMiddleware>();

            return app;
        }
    }
}