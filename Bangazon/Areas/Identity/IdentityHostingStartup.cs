using System;
using Bangazon.Areas.Identity.Data;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Bangazon.Areas.Identity.IdentityHostingStartup))]
namespace Bangazon.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<BangazonContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BangazonContextConnection")));

                //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<BangazonContext>();
            });
        }
    }
}