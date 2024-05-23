using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebClient.AppContext;
using WebClient.Models.DataModel;
namespace WebApplication1
{
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
                opt.ClientId = "your yandex key";
                opt.ClientSecret = "your yandex secret";
            })
            .AddGoogle(opt =>
            {
                opt.CallbackPath = "/signin-google";
                opt.ClientId = "your google key";
                opt.ClientSecret = "your google sercret";
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
