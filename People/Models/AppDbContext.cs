using Microsoft.EntityFrameworkCore;


namespace People.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Person> People { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
    }
}
