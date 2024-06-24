using DominosDriverHustleComp.Server.Data;
using DominosDriverHustleComp.Server.Services;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;

namespace DominosDriverHustleComp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://gps-dashboard.dominos.com.au", "https://dip.drivosity.com")
                    .AllowAnyHeader()
                    .WithMethods("POST")
                    .AllowCredentials();
                });
            });

            builder.Services.AddHostedService<GPSDashboardService>();
            builder.Services.AddHostedService((sp) => sp.GetRequiredService<GPSSSEService>());
            builder.Services.AddSingleton<GPSSSEService>();
            builder.Services.AddSingleton<HustleTracker>();
            builder.Services.AddDbContext<HustleCompContext>();
            builder.Services.AddSingleton<SummaryGeneratorService>();
            builder.Services.AddSingleton<ScreenshotService>();
            builder.Services.AddSingleton<OverspeedsService>();
            builder.Services.AddSendGrid(options => options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HustleCompContext>();
                context.Database.Migrate();
            }

            // Generate weekly database entries
            var now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                using var scope = app.Services.CreateScope();
                var generator = scope.ServiceProvider.GetRequiredService<SummaryGeneratorService>();
                generator.GenerateSummaries();

                var overspeedService = scope.ServiceProvider.GetRequiredService<OverspeedsService>();
                overspeedService.FetchOverspeeds();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseAuthorization();
            app.UseCors();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
