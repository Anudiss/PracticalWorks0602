using Microsoft.EntityFrameworkCore;

namespace Practice2.Models;

public class Context : DbContext
{
    private static Context _context;
    public static Context Entities => _context ??= new();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDb;Database=Practice2;Trusted_Connection=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }

    public static void CancelChanges()
    {
        var changedEntries = Entities.ChangeTracker.Entries()
            .Where(x => x.State != EntityState.Unchanged).ToList();

        foreach (var entry in changedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }

}
