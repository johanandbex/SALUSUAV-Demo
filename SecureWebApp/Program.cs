using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SALUSUAV_DEMO;

namespace SALUSUAVWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .Build();
    }
}
