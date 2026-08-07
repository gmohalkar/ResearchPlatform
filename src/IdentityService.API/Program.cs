using System.Text;
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
builder.Host.UseSerilog();
#region Controllers

builder.Services.AddControllers();

#endregion

#region Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
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

#endregion

#region MediatR

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<LoginUserCommand>();
});

#endregion

#region JWT Configuration

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings =
    builder.Configuration
           .GetSection("Jwt")
           .Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new Exception("Jwt configuration is missing.");
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

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    $"AUTH FAILED: {context.Exception.Message}");

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("TOKEN VALIDATED");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy(
            "CreateUser",
            policy =>
            {
                policy.Requirements.Add(
                    new PermissionRequirement(
                        "CreateUser"));
            });

        options.AddPolicy(
            "DeleteUser",
            policy =>
            {
                policy.Requirements.Add(
                    new PermissionRequirement(
                        "DeleteUser"));
            });

        options.AddPolicy(
            "ManageRoles",
            policy =>
            {
                policy.Requirements.Add(
                    new PermissionRequirement(
                        "ManageRoles"));
            });

        options.AddPolicy(
            "ViewAuditLogs",
            policy =>
            {
                policy.Requirements.Add(
                    new PermissionRequirement(
                        "ViewAuditLogs"));
            });
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
// RecurringJob.AddOrUpdate<
//     CleanupJob>(
//         "cleanup-job",
//         x => x.Execute(),
//         Cron.Daily);
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

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();

#endregion

app.Run();