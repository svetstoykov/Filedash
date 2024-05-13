using System.Data.Common;
using Filedash.Infrastructure.DbContext;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Filedash.IntegrationTests.Settings;

public class FiledashWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<FiledashDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<FiledashDbContext>((serviceProvider, opt) =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                
                opt.UseSqlServer(config.GetConnectionString("FiledashTestConnectionString"));
            });
        });

        builder.UseEnvironment("Development");
    }
}