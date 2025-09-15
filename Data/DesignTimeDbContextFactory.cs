using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RankingPadelAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost;Database=RankingPadelDB;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
