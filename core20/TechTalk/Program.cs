using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Filter;
using NetCoreWeb.Extensions;

namespace TechTalk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.UseHttps(@"C:\cert.pfx", "A");
                    options.NoDelay = true;

                    //I use this to get rid of SSL errors, feel free to remove it.
                    IConnectionFilter prevFilter = options.ConnectionFilter ?? new NoOpConnectionFilter();
                    options.ConnectionFilter = new IgnoreSslErrorsConnectionFilter(prevFilter);
                }
                        )
                        .UseUrls("https://provider2016.ibm.com:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
