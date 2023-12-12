
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.DB
{
    public class PostgresDataContext : DataContext
    {
        public PostgresDataContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
