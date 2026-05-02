using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WexCardApi.DB;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all EF/DbContext registrations
            var descriptors = services
                .Where(d => d.ServiceType.FullName!.Contains("EntityFrameworkCore")
                    || d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                    || d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            // Add in-memory database instead
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDB"));
        });
    }
}
