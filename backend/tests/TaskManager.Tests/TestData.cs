using TaskManager.Api.Data;
using TaskManager.Api.Entities;

namespace TaskManager.Tests;

public static class TestData
{
    public static AppUser User(Guid id) => new()
    {
        Id = id,
        UserName = $"user-{id}",
        NormalizedUserName = $"USER-{id}".ToUpperInvariant(),
        Email = $"{id}@test.local",
        NormalizedEmail = $"{id}@TEST.LOCAL".ToUpperInvariant(),
        CreatedAt = DateTimeOffset.UtcNow
    };

    public static Project Project(Guid id, Guid ownerId, string name = "Project") => new()
    {
        Id = id,
        Name = name,
        OwnerId = ownerId,
        CreatedAt = DateTimeOffset.UtcNow
    };

    public static TaskItem Task(
        Guid id,
        Guid projectId,
        TaskItemStatus status = TaskItemStatus.Todo,
        TaskPriority priority = TaskPriority.Medium) => new()
    {
        Id = id,
        Title = "Task",
        Status = status,
        Priority = priority,
        ProjectId = projectId,
        CreatedAt = DateTimeOffset.UtcNow
    };

    public static async Task SeedUsersAsync(SqliteInMemoryDatabase db, params Guid[] ids)
    {
        await using var context = db.CreateContext();
        foreach (var id in ids)
            context.Users.Add(User(id));
        await context.SaveChangesAsync();
    }
}
