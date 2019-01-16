using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.File;
using Ocelot.Extensions.Configuration;

namespace PollingConfigurationFromAzure
{
    public class Program
    {
        public static void Main(string[] args)
        {         
            new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonFile("ocelot.json", false, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, s) => {
                s.AddOcelot()
                    .WithConfigurationRepository();                
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                //add your logging
            })
            .UseIISIntegration()

            .Configure(app =>
            {                
                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
        }

    }
}
