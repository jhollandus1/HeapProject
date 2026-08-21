using Microsoft.EntityFrameworkCore;

public class HeapProjectContext(DbContextOptions<HeapProjectContext> options) : DbContext(options)
{
    public DbSet<HeapProject.Models.HeapHistoryItem> HeapHistoryItem { get; set; } = default!;
}
