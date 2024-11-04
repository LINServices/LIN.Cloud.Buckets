using LIN.Types.Cloud.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Identity.Persistence.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{


    public DbSet<BucketModel> Buckets { get; set; }
    public DbSet<BucketAccessModel> BucketAccess { get; set; }
    public DbSet<BucketIdentityModel> BucketIdentities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BucketModel>(entity =>
        {
            entity.ToTable("buckets");
        });

      
     
        modelBuilder.Entity<BucketAccessModel>(entity =>
        {
            entity.ToTable("bucket_access");
            entity.HasOne(g => g.Bucket)
                  .WithMany()
                  .HasForeignKey(g => g.BucketId)
                  .OnDelete(DeleteBehavior.NoAction);

        });

        modelBuilder.Entity<BucketIdentityModel>(entity =>
        {
            entity.ToTable("bucket_identities");
            entity.HasOne(g => g.BucketModel)
                  .WithMany()
                  .HasForeignKey(g => g.BucketId)
                  .OnDelete(DeleteBehavior.NoAction);

        });

        base.OnModelCreating(modelBuilder);
    }

}