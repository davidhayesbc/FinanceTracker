using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    private static readonly Dictionary<string, string> VersionInformation;

    static Extensions()
    {
        VersionInformation = new Dictionary<string, string>
        {
            { "applicationVersion", typeof(Extensions).Assembly.GetName().Version?.ToString() ?? "unknown" },
            { "dotnetVersion", System.Environment.Version.ToString() }
        };
    }

    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry((OpenTelemetryLoggerOptions logging) =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics((MeterProviderBuilder metrics) =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing((TracerProviderBuilder tracing) =>
            {
                //Filter out health check requests from tracing, they fill the logs with noise
                tracing.AddAspNetCoreInstrumentation((o) => o.Filter = (HttpContext context) => !context.Request.Path.Equals("/health"));
                tracing.AddHttpClientInstrumentation();
                tracing.AddSqlClientInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.EnableConnectionLevelAttributes = true;
                    options.SetDbStatementForStoredProcedure = true;
                    options.RecordException = true;
                    //options.Filter = new Func<object, bool>((activity) =>
                    //{
                    //    SqlCommand cmd = activity as SqlCommand;
                    //    // Exclude health check activities
                    //    var b = !cmd.CommandText.Equals("SELECT 1");

                    //    return b;
                    //});
                });
            });


        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        
        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        results = report.Entries.Select(entry => new
                        {
                            key = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            data = entry.Value.Data
                        }),
                        versionInfo = VersionInformation
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
