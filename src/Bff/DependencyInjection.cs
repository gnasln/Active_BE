using Bff.Application.Common.Interfaces;
using Bff.Application.Contracts.Persistence;
using Bff.Infrastructure.Data;
using Bff.Infrastructure.Extensions;
using Bff.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using PerfSvc.Application.Object.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IMailServices, MailServices>();
        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .Build())
            .AddPolicy("OpenIddict.Server.AspNetCore", policy =>
            {
                policy.AuthenticationSchemes.Add(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("admin", policy =>
            {
                policy.AuthenticationSchemes = [OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme];
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Administrator");
            });
            
        //Register AuthorizationChecker
        services.AddTransient<AuthorizationChecker>();

        return services;
    }
}
