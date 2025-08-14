using FluentValidation;
using FluentValidation.AspNetCore;
using CorePackages.Infrastructure.Authentication;
using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Interfaces;
using CorePackages.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;
using CorePackages.Infrastructure.Extentions;

namespace CorePackages.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextService, HttpContextService>();

            services.AddMemoryCache();

            services.AddSingleton<ICacheService, CacheService>();

            services.AddScoped<IMediatrService, MediatrService>();

            services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            AddVault(services, configuration);

            AddAuthenticationAuthorization(services);
        }
        private static void AddVault(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IConfigurationHelper<>), typeof(ConfigurationHelper<>));
            services.AddMemoryCache();

            var vaultAddress = Environment.GetEnvironmentVariable("VaultAddress") ?? configuration["VaultSettings:Address"];
            var vaultRoleId = Environment.GetEnvironmentVariable("VaultRoleId") ?? configuration["VaultSettings:RoleId"];
            var vaultSecretId = Environment.GetEnvironmentVariable("VaultSecretId") ?? configuration["VaultSettings:SecretId"];

            VaultClient vaultClient = new(new HttpClient
            {
                BaseAddress = new Uri(vaultAddress)
            }, vaultRoleId, vaultSecretId);

            services.AddHttpClient<IVaultClient, VaultClient>(client => vaultClient);
        }

        private static void AddAuthenticationAuthorization(IServiceCollection services)
        {
            var keycloakConfig = services.BuildServiceProvider().GetRequiredService<IConfigurationHelper<KeycloakConfig>>().GetConfigurationAsync("KeycloakConfig").GetAwaiter().GetResult();

            services.AddHttpClient("KeycloakClient", c =>
            {
                c.BaseAddress = new Uri(keycloakConfig.BaseAddress);
            });

            services.AddHttpClient<IKeycloakService, KeycloakService>(client =>
            {
                client.BaseAddress = new Uri(keycloakConfig.BaseAddress);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakConfig.Authority;
                options.Audience = keycloakConfig.Audience;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = keycloakConfig.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = keycloakConfig.Authority,
                    ValidateLifetime = true
                };
            });

            services.AddAuthorization(opts =>
            {
                foreach (var scope in keycloakConfig.Scopes)
                {
                    opts.AddPolicy($"{scope.FirstCharToUpper()}Scope", policy =>
                    {
                        policy.Requirements.Add(new HasScopeRequirement(scope));
                    });
                }
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeRequerementAuthHandler>();
        }
    }
}
