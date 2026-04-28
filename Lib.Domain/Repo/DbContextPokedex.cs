using Microsoft.EntityFrameworkCore;

namespace Lib.Domain.Repo;

public class DbContextPokedex : DbContext
{
    /*public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Recipe_Ingredient> Recipes_Ingredients { get; set; }
    public DbSet<Recipe_Label> Recipes_Label { get; set; }
    public DbSet<Recipe_Category> Recipes_Categories { get; set; }

    public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        //this.Database.EnsureCreated();
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is DomainElement && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((DomainElement)entityEntry.Entity).UpdateDate = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((DomainElement)entityEntry.Entity).InsertDate = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }*/
}