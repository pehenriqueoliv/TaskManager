using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskManager.Api.Data;

namespace TaskManager.Tests;

public sealed class SqliteInMemoryDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public SqliteInMemoryDatabase(bool enforceForeignKeys = true)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .ReplaceService<IModelCustomizer, SqliteModelCustomizer>()
            .Options;

        using (var context = CreateContext())
            context.Database.EnsureCreated();

        using var command = _connection.CreateCommand();
        command.CommandText = enforceForeignKeys ? "PRAGMA foreign_keys=ON;" : "PRAGMA foreign_keys=OFF;";
        command.ExecuteNonQuery();
    }

    public AppDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
