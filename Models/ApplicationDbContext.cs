using Microsoft.EntityFrameworkCore;

namespace cvicenie_mvc.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentModel> Students { get; set; }
    }
}

