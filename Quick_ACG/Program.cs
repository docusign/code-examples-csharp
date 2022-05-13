using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DocuSign.QuickACG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(Path.GetFullPath(@"..\\launcher-csharp\\appsettings.json"),
                                   optional: false,
                                   reloadOnChange: true);
            });
            return builder;
        }
    }
}
