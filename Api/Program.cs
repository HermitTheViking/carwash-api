using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using SerilogWeb.Classic.Enrichers;
using System;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var applicationName = "Carwash";
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.With<HttpRequestIdEnricher>()
                .Enrich.With<HttpRequestRawUrlEnricher>()
                .Enrich.With<HttpRequestTypeEnricher>()
                .Enrich.With<HttpRequestUrlReferrerEnricher>()
                .Enrich.With<HttpRequestUrlEnricher>()
                .Enrich.With<HttpRequestUserAgentEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("EnvironmentName", environmentName)
                .WriteTo.Console()
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
.UseSerilog()
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
});
        }
    }
}