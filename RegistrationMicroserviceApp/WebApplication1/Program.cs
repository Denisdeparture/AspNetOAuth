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
                opt.ClientId = "7159bbb6f77f44a7aa8c1aa2cd0f05af";
                opt.ClientSecret = "76b0d716fd004fe88104d9506029e699";
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
