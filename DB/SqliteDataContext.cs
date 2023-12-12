using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace WebApplication1.DB
{
    public class SqliteDataContext : DataContext
    {
        public SqliteDataContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
