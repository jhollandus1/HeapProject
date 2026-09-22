using Microsoft.EntityFrameworkCore;

public class HeapProjectContext(DbContextOptions<HeapProjectContext> options) : DbContext(options)
{
    public virtual DbSet<HeapProject.Models.HeapHistoryItem> HeapHistoryItem { get; set; } = default!;

    public override int SaveChanges()
    {
        UpdateTimestamp();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamp();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamp()
    {
        var entries = ChangeTracker.Entries<HeapProject.Models.HeapHistoryItem>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach(var entry in entries)
        {
            entry.Entity.SavedTime = DateTime.UtcNow;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HeapProject.Models.HeapHistoryItem>()
                        .Property(e => e.SavedTime)
                        .HasColumnType("datetime2");
    }
}
