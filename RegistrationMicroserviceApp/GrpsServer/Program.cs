using GrpsServer.Services;
using Microsoft.EntityFrameworkCore;
using WebClient.AppContext;
using WebClient.Models.DataModel;

namespace GrpsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationContexts>((users) => users.UseSqlServer(@"Server= WIN-Q0NS67721NA\\SQLEXPRESS;Database=ApplicationAuthorizationDataBase;Trusted_Connection=True; TrustServerCertificate=True;"));
            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddDefaultIdentity<DataUserModel>(opt => opt.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContexts>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<ConsistencyService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}