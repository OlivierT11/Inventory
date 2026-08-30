using Inventory.Data;
using Inventory.Models;
using Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Tests;

public class UnitTest1
{
    private static AppDbContext CreateContext()
    {
        // Configure Entity Framework Core pour utiliser une base de données stockée en mémoire (UseInMemoryDatabase)
        // Avec UseInMemoryDatabase, aucune donnée n’est ajoutée à la vraie base de données. Les données sont stockées uniquement dans une base simulée en mémoire :
        // Le nom de la base est généré ainsi, Par exemple : a7f4a3e7 - 4ec9 - 4a32 - 9dc5 - 18d9d32c9d11
        // Cela donne une base différente à chaque appel de CreateContext().
        // C’est important dans les tests, car les données d’un test ne se mélangent pas avec celles d’un autre test.
        // Point important : UseInMemoryDatabase est pratique pour des tests simples, mais ce n’est pas exactement le même comportement qu’une vraie base SQL.
        // Pour tester les requêtes SQL, les contraintes ou les transactions, SQLite en mémoire est souvent plus représentatif.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_GetById_ReturnsCreated()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);

        var product = new Product { Name = "Widget", Price = 9.99m, Stock = 5 };
        var created = await repo.CreateAsync(product);

        Assert.True(created.Id > 0);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Widget", fetched!.Name);
        Assert.Equal(9.99m, fetched.Price);
        Assert.Equal(5, fetched.Stock);
    }

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);
        await repo.CreateAsync(new Product { Name = "A", Price = 1m, Stock = 1 });
        await repo.CreateAsync(new Product { Name = "B", Price = 2m, Stock = 2 });

        var list = (await repo.GetAllAsync()).ToList();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task Update_ExistingProduct_UpdatesFields()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);
        var created = await repo.CreateAsync(new Product { Name = "Old", Price = 1m, Stock = 1 });

        created.Name = "New";
        created.Price = 2m;
        created.Stock = 3;

        var updated = await repo.UpdateAsync(created);
        Assert.NotNull(updated);
        Assert.Equal("New", updated!.Name);
        Assert.Equal(2m, updated.Price);
        Assert.Equal(3, updated.Stock);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);
        var product = new Product { Id = 999, Name = "X", Price = 1m, Stock = 1 };
        var result = await repo.UpdateAsync(product);
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);
        var created = await repo.CreateAsync(new Product { Name = "ToDelete", Price = 1m, Stock = 1 });

        var deleted = await repo.DeleteAsync(created.Id);
        Assert.True(deleted);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsFalse()
    {
        using var context = CreateContext();
        var repo = new ProductRepository2(context);

        var deleted = await repo.DeleteAsync(999);
        Assert.False(deleted);
    }
}
