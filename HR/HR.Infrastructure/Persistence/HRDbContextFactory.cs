using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HR.Infrastructure.Persistence;

public class HRDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
{
    public HRDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=HRDb;User Id=sheik;Password=sheik@123;TrustServerCertificate=True;");
        return new HRDbContext(optionsBuilder.Options);
    }
}