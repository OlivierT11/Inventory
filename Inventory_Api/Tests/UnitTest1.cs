using Inventory.Data;
using Inventory.Models;
using Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

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

    /// <summary>
    /// Test to ensure that the GetByIdAsync method returns the correct product when it exists in the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ReturnsProduct()
    {
        using var context = CreateContext();
        // NullLogger<T> is appropriate when the test is testing repository behavior, not logging behavior.
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var product = new Product { Name = "Widget", Price = 9.99m, Stock = 5 };
        var created = await repo.CreateAsync(product);

        Assert.True(created.Id > 0);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Widget", fetched!.Name);
        Assert.Equal(9.99m, fetched.Price);
        Assert.Equal(5, fetched.Stock);
    }

    /// <summary>
    /// Test to ensure that the GetByIdAsync method returns null when the product does not exist in the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_WhenNoProductsExist_ReturnsNull()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var fetched = await repo.GetByIdAsync(99999); // ID that doesn't exist
        Assert.Null(fetched);
    }

    /// <summary>
    /// Test to ensure that the GetByIdAsync method respects cancellation tokens.
    /// Timeouts should only be tested if business critical.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_WhenCallerCancels_ThrowsOperationCanceledException()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var product = new Product { Name = "Widget", Price = 9.99m, Stock = 5 };
        var created = await repo.CreateAsync(product);

        // pass an already-canceled token and verify that the method throws OperationCanceledException
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repo.GetByIdAsync(
                created.Id,
                cancellationTokenSource.Token));
    }

    /// <summary>
    /// Test to ensure that GetAllAsync returns all products in the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);
        await repo.CreateAsync(new Product { Name = "A", Price = 1m, Stock = 1 });
        await repo.CreateAsync(new Product { Name = "B", Price = 2m, Stock = 2 });

        var list = (await repo.GetAllAsync()).ToList();
        Assert.Equal(2, list.Count);
    }

    /// <summary>
    /// Test to ensure that GetAllAsync returns an empty collection when no products exist in the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyCollection()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var products = await repo.GetAllAsync();

        Assert.NotNull(products);
        Assert.Empty(products);
    }

    /// <summary>
    /// Test to ensure that GetAllAsync respects cancellation tokens.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_WhenCallerCancels_ThrowsOperationCanceledException()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repo.GetAllAsync(cts.Token));
    }

    /// <summary>
    /// Do not test logging unless logging itself is a requirement.
    /// </summary>
    /// <returns></returns>
    //[Fact]
    //public async Task GetAllAsync_LoggingTest()
    //{
    //    //empty test to ensure that logging does not throw exceptions
    //}

    /// <summary>
    /// Test to ensure that updating an existing product correctly updates its fields in the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateAsync_ExistingProduct_UpdatesFields()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);
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

    /// <summary>
    /// Test to ensure that updating a non-existing product returns null.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsNull()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);
        var product = new Product { Id = 999, Name = "X", Price = 1m, Stock = 1 };
        var result = await repo.UpdateAsync(product);
        Assert.Null(result);
    }

    /// <summary>
    /// Test to ensure that updating a product respects cancellation tokens.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateAsync_WhenCallerCancels_ThrowsOperationCanceledException()
    {
        using var context = CreateContext();

        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var created = await repo.CreateAsync(new Product
        {
            Name = "Old",
            Price = 1m,
            Stock = 1
        });

        created.Name = "New";
        created.Price = 2m;
        created.Stock = 3;

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repo.UpdateAsync(
                created,
                cancellationTokenSource.Token));
    }


    /// <summary>
    /// Test to ensure that deleting a product actually removes it from the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);
        var created = await repo.CreateAsync(new Product { Name = "ToDelete", Price = 1m, Stock = 1 });

        var deleted = await repo.DeleteAsync(created.Id);
        Assert.True(deleted);

        var fetched = await repo.GetByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    /// <summary>
    /// Test to ensure that deleting a non-existing product returns false.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        using var context = CreateContext();
        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var deleted = await repo.DeleteAsync(999);
        Assert.False(deleted);
    }

    /// <summary>
    /// Test to ensure that deleting a product respects cancellation tokens.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteAsync_WhenCallerCancels_ThrowsOperationCanceledException()
    {
        using var context = CreateContext();

        var logger = NullLogger<ProductRepository2>.Instance;
        var repo = new ProductRepository2(context, logger);

        var created = await repo.CreateAsync(new Product
        {
            Name = "ToDelete",
            Price = 1m,
            Stock = 1
        });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repo.DeleteAsync(
                created.Id,
                cancellationTokenSource.Token));
    }


}
