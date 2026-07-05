using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cico.Infrastructure.Persistence;

public class CICODbContextFactory
    : IDesignTimeDbContextFactory<CICODbContext>
{
    public CICODbContext CreateDbContext(
        string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<CICODbContext>();

        optionsBuilder.UseSqlServer(
            @"Server=TANTRAN\SQLEXPRESS;
              Database=CICO;
              Trusted_Connection=True;
              TrustServerCertificate=True");

        return new CICODbContext(
            optionsBuilder.Options);
    }
}