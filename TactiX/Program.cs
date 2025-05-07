using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TactiX.DBContext;
using TactiX.Models;
using TactiX.Services;
using System.Net.Http;


namespace TactiX
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllersWithViews()
            .AddJsonOptions(options =>
                 options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IEmailService, ElasticEmailService>();
            builder.Services.AddScoped<MatchAnalysisService>();
            builder.Services.AddScoped<TrainingAnalysisService>();

            builder.Services.AddDbContext<TactiXDB>(options =>
                 options.UseNpgsql(builder.Configuration.GetConnectionString("TactiXDB")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/Error";
                    await next();
                }
            });

            app.Run();
        }
    }
}