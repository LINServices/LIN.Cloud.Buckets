using LIN.Types.Cloud.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Identity.Persistence.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{


    public DbSet<BucketModel> Buckets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BucketModel>(entity =>
        {
            entity.ToTable("buckets");
            entity.HasIndex(t=>t.ProjectId).IsUnique();
        });

    

        base.OnModelCreating(modelBuilder);
    }

}