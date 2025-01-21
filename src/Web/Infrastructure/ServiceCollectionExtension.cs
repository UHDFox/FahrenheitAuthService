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
            options.AddPolicy("SomePolicy", policyBuilder =>
            {
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyOrigin();
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
            .WriteTo.Async(a => a.File("/app/logs/log.txt", rollingInterval: RollingInterval.Day))
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.AddSerilog();

            logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore.DataProtection",
                LogLevel.Error); // Suppress data protection warnings
            logging.AddFilter("Microsoft.AspNetCore.Diagnostics", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Warning);

            logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });
    }
}