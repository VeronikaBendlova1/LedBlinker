using System.Globalization;
using LedBlinker.Data;
using LedBlinker.Repository;
using LedBlinker.Repository.Impl;
using LedBlinker.Service;
using LedBlinker.Service.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



namespace LedBlinker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            builder.Services.AddScoped<ILedStateService, LedStateService>();
            builder.Services.AddScoped<ILedRepo, LedRepo>();
            builder.Services.AddScoped<ILogRepo, LogRepository>();
            builder.Services.AddScoped<IConfigurationRepo, ConfigurationRepo>();
            builder.Services.AddScoped<IConfigurationServiceDefault, ConfigurationServiceDefault>();

            if (builder.Environment.EnvironmentName == "Custom")
            {
                builder.Services.AddScoped<ILogServiceDefault, LogServiceCustom>();
            }
            else
            {
                builder.Services.AddScoped<ILogServiceDefault, LogServiceDefault>();
            }
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "LedBlinker API",
                    Version = "v1"
                });
            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LedBlinker API v1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Přidá mapování pro API controllery - aby vůbec fungovalo API
            app.MapControllers();

            // Pro případné MVC stránky
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=LedView}/{action=State}/{id?}");

            app.Run();
        }
    }
}
