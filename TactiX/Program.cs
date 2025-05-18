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
        public static async Task Main(string[] args) // �������� async
        {
            var builder = WebApplication.CreateBuilder(args);

            // �������� ������ �����������
            var connectionString = builder.Configuration.GetConnectionString("TactiXDB");

            // ����������� DbContext (���� ���!)
            builder.Services.AddDbContext<TactiXDB>(options =>
                options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure()));

            // ��������� �������
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IEmailService, ElasticEmailService>();
            builder.Services.AddScoped<MatchAnalysisService>();
            builder.Services.AddScoped<TrainingAnalysisService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            var app = builder.Build();

            // ��������� ��������
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TactiXDB>();
                await db.Database.MigrateAsync(); // ������ � await
            }

            // ��������� ������������
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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

            await app.RunAsync(); // �������� �� RunAsync
        }
    }
}