using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Web.Components;
using Web.Components.Account;
using Web.Configuration;
using Web.Data;
using Web.Services;

namespace Web.Extensions;

public static class ProgramBuilderExtensions
{
    public static IServiceCollection RegisterDataComponents(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString( nameof(ConnectionStrings.DefaultConnection)) ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString);
            //.UseAsyncSeeding()
        });
            
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        return services;
    }
    
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
        
        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole",
                policy => policy.RequireRole("Administrator"));
        });
        
        // services.Configure<IdentityOptions>(options =>
        // {
        //     // Password settings.
        //     options.Password.RequireDigit = true;
        //     options.Password.RequireLowercase = true;
        //     options.Password.RequireNonAlphanumeric = true;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequiredLength = 6;
        //     options.Password.RequiredUniqueChars = 1;
        //
        //     // Lockout settings.
        //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //     options.Lockout.MaxFailedAccessAttempts = 5;
        //     options.Lockout.AllowedForNewUsers = true;
        //
        //     // User settings.
        //     options.User.AllowedUserNameCharacters =
        //         "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        //     options.User.RequireUniqueEmail = false;
        // });
        //
        // // TODO: Unsure if this is necessary - Seems to all work be default without this
        // services.ConfigureApplicationCookie(options =>
        // {
        //     // Cookie settings
        //     options.Cookie.HttpOnly = true;
        //     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        //
        //     options.LoginPath = "/Identity/Account/Login";
        //     options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        //     options.SlidingExpiration = true;
        // });

        return services;
    }
    
    public static IServiceCollection RegisterWebComponents(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureOptions(services, configuration);
        RegisterServices(services);
        RegisterRazor(services);
        
        return services;
    }
    
    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionStrings>(configuration.GetSection(ConnectionStrings.SectionName));
        
        services.Configure<SendGridConfig>(configuration.GetSection(SendGridConfig.SectionName));
        services.Configure<GmailConfig>(configuration.GetSection(GmailConfig.SectionName));
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IEmailSender<ApplicationUser>, EmailSender>();
        services.AddScoped<IMailService, SendGridMailService>();
    }

    private static void RegisterRazor(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
    }
}
