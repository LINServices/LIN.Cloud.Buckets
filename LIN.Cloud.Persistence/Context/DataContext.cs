using LIN.Types.Cloud.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Persistence.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<BucketModel> Buckets { get; set; }
    public DbSet<PublicFileModel> PublicFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BucketModel>(entity =>
        {
            entity.ToTable("buckets");
            entity.HasIndex(t => t.ProjectId).IsUnique();
        });

        modelBuilder.Entity<PublicFileModel>(entity =>
        {
            entity.ToTable("public_files");
            entity.HasKey(t => t.Key);
        });

        base.OnModelCreating(modelBuilder);
    }

}