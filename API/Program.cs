using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entities;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           var host= CreateHostBuilder(args).Build();
           using var scope=host.Services.CreateScope();
           var serviceProvider=scope.ServiceProvider;
           try
           {
               var dbContext=serviceProvider.GetRequiredService<DataContext>();
               var userManager=serviceProvider.GetRequiredService<UserManager<AppUser>>();
               var roleManager=serviceProvider.GetRequiredService<RoleManager<AppRole>>();
               await dbContext.Database.MigrateAsync();
               await Seed.SeedUserData(userManager,roleManager);
           }
           catch(Exception ex)
           {
               var logger=serviceProvider.GetRequiredService<ILogger<Program>>();
               logger.LogError(ex,ex.Message);
           }
           await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}