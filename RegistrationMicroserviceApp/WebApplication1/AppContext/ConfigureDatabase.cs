using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebClient.Models.DataModel;

namespace WebClient.AppContext
{
    public class ConfigureDatabase : IEntityTypeConfiguration<DataUserModel>
    {
        public void Configure(EntityTypeBuilder<DataUserModel> builder)
        {
            foreach(var prop in builder.GetType().GetProperties())
            {
                if (prop.Name == "Email" | prop.Name == "PasswordHash") continue;
                builder.Ignore(p => prop.GetValue(prop));
            }
        }

    }
}
