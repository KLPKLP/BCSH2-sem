using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace BCSH2_SEM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var defaultCulture = new CultureInfo("cs-CZ");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };


            // Culture info:
            var cultureInfo = new CultureInfo("cs-CZ");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(); //povolit session

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession(); //povolit session
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
            // pattern: "{controller=Home}/{action=Index}/{id?}");
             pattern: "{controller=Auth}/{action=Login}");


            app.UseRequestLocalization(localizationOptions);

            app.Run();
        }
    }
}
