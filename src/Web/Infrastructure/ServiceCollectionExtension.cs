using Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;

namespace Web.Infrastructure;

public static class ServiceCollectionExtension
{
    public static void AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<FahrenheitAuthDbContext>((provider, builder) =>
        {
            builder.UseNpgsql(provider.GetRequiredService<IConfiguration>().GetConnectionString("Psql"));
            builder.ConfigureWarnings(
                warnings => warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
        });
    }

    public static void TestDbConnection(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FahrenheitAuthDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
            try
            {
                logger.LogInformation("Testing database connection...");
                dbContext.Database.OpenConnection();
                dbContext.Database.CloseConnection();
                logger.LogInformation("✅ Successfully connected to the database!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Failed to connect to the database.");
            }
        }
    }

    public static void ConfigureStaticFilesUpload(this IApplicationBuilder app)
    {
        var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(uploadsFolderPath)) Directory.CreateDirectory(uploadsFolderPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsFolderPath),
            RequestPath = "/uploads"
        });
    }

    public static void ConfigureCORSPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AuthCORSPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:7045", "http://localhost:5000")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void AddSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .WriteTo.Async(a => a.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"))
            .WriteTo.Async(a => a.File("/app/logs/authlog.txt", rollingInterval: RollingInterval.Day))
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.AddSerilog();
            logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });
    }
    public static void ApplyMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<FahrenheitAuthDbContext>();
            dbContext.Database.Migrate(); // Applies any pending migrations
            Log.Information("Database migration applied successfully.");
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while applying the database migration: {ex.Message}");
        }
    }
}