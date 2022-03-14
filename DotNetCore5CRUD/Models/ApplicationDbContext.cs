using Microsoft.EntityFrameworkCore;

namespace DotNetCore5CRUD.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}