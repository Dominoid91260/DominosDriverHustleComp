using DominosDriverHustleComp.Data;
using DominosDriverHustleComp.Services;

using Microsoft.EntityFrameworkCore;

namespace DominosDriverHustleComp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

#if DEBUG
            builder.Logging.AddDebug();
#endif // DEBUG

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<HustleCompContext>();
            builder.Services.AddSingleton<GPSService>();
            builder.Services.AddSingleton<IHostedService, GPSService>(serviceProvider => serviceProvider.GetService<GPSService>());
            //builder.Services.AddHostedService<DummyGPSService>();
            builder.Services.AddSingleton<DeliveryTrackerService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();
                context.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapGet("/gpstoken", (string bearerToken, GPSService gpsService) =>
            {
                gpsService.BearerToken = bearerToken;
            });

            app.Run();
        }
    }
}
