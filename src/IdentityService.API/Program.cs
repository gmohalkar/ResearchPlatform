using System.Text;
using Azure.Identity;
using FluentValidation;
using Hangfire;
using IdentityService.API.Authorization;
using IdentityService.API.Middleware;
using IdentityService.Application.Common;
using IdentityService.Application.Features.Authentication.LoginUser;
using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Reader;
using Serilog;

Log.Logger =
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(
            "Logs/log-.txt",
            rollingInterval:
            RollingInterval.Day)
        .CreateLogger();
var builder = WebApplication.CreateBuilder(args);

var keyVaultUri =
    builder.Configuration["KeyVaultUri"];

if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration
        .AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
}
//builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString =
        "InstrumentationKey=eb1baab3-b83e-4fb0-9f9c-d77a80b56108;IngestionEndpoint=https://centralindia-0.in.applicationinsights.azure.com/;LiveEndpoint=https://centralindia.livediagnostics.monitor.azure.com/;ApplicationId=9a583470-6629-4af5-97f3-f9be7b670f78";
});
Console.WriteLine("AI CONFIGURED");
builder.Host.UseSerilog();
#region Controllers

builder.Services.AddControllers();

#endregion

#region Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

#endregion

#region Fluent Validation
builder.Services.AddValidatorsFromAssemblyContaining<
RegisterUserValidator>();

#endregion

#region Dependency Injection

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<
    IRefreshTokenRepository,
    RefreshTokenRepository>();

builder.Services.AddScoped<
    IRefreshTokenService,
    RefreshTokenService>();

    builder.Services.AddScoped<
    IResetPasswordTokenService,
    ResetPasswordTokenService>();

    builder.Services.AddScoped<
    IPermissionRepository,
    PermissionRepository>();

    builder.Services.AddSingleton<
    IAuthorizationHandler,
    PermissionHandler>();

    builder.Services.AddScoped<
    IAuditLogRepository,
    AuditLogRepository>();

    builder.Services.AddScoped<
    IAuditService,
    AuditService>();

    // builder.Services.AddScoped<ICacheService,
    // RedisCacheService>();

    builder.Services.AddScoped<CleanupJob>();

    builder.Services.AddScoped<
    IOutboxRepository,
    OutboxRepository>();

    builder.Services.AddScoped<OutboxProcessorJob>();

#endregion

#region MediatR

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<LoginUserCommand>();
});

#endregion

#region JWT Configuration
Console.WriteLine(
$"JWT Key = {builder.Configuration["Jwt:Key"]}");
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings =
    builder.Configuration
        .GetSection("Jwt")
        .Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new Exception(
        "JWT configuration is missing.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSettings.Issuer,

                ValidAudience =
                    jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key))
            };

        options.Events =
            new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine(
                        $"AUTH FAILED: {context.Exception.Message}");

                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    Console.WriteLine(
                        "TOKEN VALIDATED");

                    return Task.CompletedTask;
                }
            };
    });

#endregion
// #region Redis Configuration
// builder.Services.AddStackExchangeRedisCache(
//     options =>
//     {
//         options.Configuration =
//             builder.Configuration["Redis:ConnectionString"];

//         options.InstanceName =
//             "ResearchPlatform";
//     });
// #endregion

#region Hangfire Configuration

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(
        builder.Configuration
            .GetConnectionString(
                "DefaultConnection"));
});

builder.Services.AddHangfireServer();

#endregion
#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager =
        scope.ServiceProvider
            .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<CleanupJob>(
        "cleanup-job",
        x => x.Execute(),
        Cron.Daily);
}

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await RoleSeeder.SeedAsync(context);
    await PermissionSeeder.SeedAsync(context);
}

using(var scope =
    app.Services.CreateScope())
{
    var recurringJobs =
        scope.ServiceProvider
            .GetRequiredService<
                IRecurringJobManager>();

    recurringJobs.AddOrUpdate<
        OutboxProcessorJob>(
            "process-outbox",
            x => x.Execute(),
            Cron.Minutely);
}

#region Middleware

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(
        "Research Platform API Running Successfully");
});

#endregion
app.Run();