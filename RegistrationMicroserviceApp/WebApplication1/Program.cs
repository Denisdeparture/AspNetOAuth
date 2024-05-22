using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebClient.AppContext;
using WebClient.Models.DataModel;
namespace WebApplication1
{
//    Домашнее задание:
//1.Создать ASP.NET WebApi приложение для работы с пользователями, митингами и митинг румами. Обязательно регистрация сваггера.
//2.Добавить БД используя подход Code-First БД через миграцию.
//3.Добавить CRUD операции на сервис митинг-рума / пользователями, методы контроллера.
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddDbContext<ApplicationContexts>((users) => users.UseSqlServer(@"Server=WIN-Q0NS67721NA\SQLEXPRESS;Database=ApplicationAuthorizationDataBase;Trusted_Connection=True;TrustServerCertificate=True;"));
            builder.Services.AddDefaultIdentity<DataUserModel>(opt => opt.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContexts>();
            builder.Services.AddAuthentication()
            .AddYandex(opt =>
            {
                opt.CallbackPath = "/signin-yandex";
                opt.ClientId = "7159bbb6f77f44a7aa8c1aa2cd0f05af";
                opt.ClientSecret = "76b0d716fd004fe88104d9506029e699";
            })
            .AddGoogle(opt =>
            {
                opt.CallbackPath = "/signin-google";
                opt.ClientId = "753901439060-ah20k0qol4n8cq7cda5jhk950eufe9m5.apps.googleusercontent.com";
                opt.ClientSecret = "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";
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Home/Authentication";
               // options.AccessDeniedPath = "/";
            });
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
