using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.InteropServices;

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
                string path = string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    path = Path.GetFullPath(@"../launcher-csharp/appsettings.json");
                }
                else
                {
                    path = Path.GetFullPath(@"..\\launcher-csharp\\appsettings.json");
                }
                 

                config.AddJsonFile(path,
                                   optional: false,
                                   reloadOnChange: true);
            });
            return builder;
        }
    }
}
