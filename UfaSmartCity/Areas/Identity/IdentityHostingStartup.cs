using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(UfaSmartCity.Areas.Identity.IdentityHostingStartup))]
namespace UfaSmartCity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}