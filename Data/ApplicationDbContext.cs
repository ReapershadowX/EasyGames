using Microsoft.EntityFrameworkCore;

namespace EasyGamesProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Correctly call base constructor
        {
        }

        // Do not declare a 'Database' property here
    }
}
