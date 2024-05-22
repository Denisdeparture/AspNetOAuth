
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebClient.Models.DataModel;

namespace WebClient.AppContext
{
    public class ApplicationContexts : IdentityDbContext<DataUserModel>
    {
        public ApplicationContexts(DbContextOptions<ApplicationContexts> opt) : base(opt)
        {
            //Database.EnsureCreated();
        }
        
    }
}
