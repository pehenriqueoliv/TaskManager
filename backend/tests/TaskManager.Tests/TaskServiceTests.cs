using TaskManager.Api.Common;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsTask_WhenValid()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        await SeedProjectAsync(db, owner, projectId);

        var request = new CreateTaskRequest("Escrever testes", "com xUnit",
            TaskItemStatus.InProgress, TaskPriority.High, DateTimeOffset.UtcNow.AddDays(3));

        TaskResponse response;
        await using (var context = db.CreateContext())
            response = await new TaskService(context).CreateAsync(owner, projectId, request);

        Assert.Equal(TaskItemStatus.InProgress, response.Status);

        await using var check = db.CreateContext();
        var stored = Assert.Single(check.Tasks);
        Assert.Equal("Escrever testes", stored.Title);
        Assert.Equal(projectId, stored.ProjectId);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDueDateIsInThePast()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        await SeedProjectAsync(db, owner, projectId);

        var request = new CreateTaskRequest("Atrasada", null,
            TaskItemStatus.Todo, TaskPriority.Low, DateTimeOffset.UtcNow.AddDays(-1));

        await using var context = db.CreateContext();
        var service = new TaskService(context);

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(owner, projectId, request));

        await using var check = db.CreateContext();
        Assert.Empty(check.Tasks);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenProjectBelongsToAnotherUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var intruder = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner, intruder);
        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(projectId, owner));
            await seed.SaveChangesAsync();
        }

        var request = new CreateTaskRequest("Invasora", null,
            TaskItemStatus.Todo, TaskPriority.Low, null);

        await using var context = db.CreateContext();
        var service = new TaskService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(intruder, projectId, request));
    }

    [Fact]
    public async Task GetAllAsync_FiltersByStatusAndPriority()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        await SeedProjectAsync(db, owner, projectId);

        await using (var seed = db.CreateContext())
        {
            seed.Tasks.Add(TestData.Task(Guid.NewGuid(), projectId, TaskItemStatus.Todo, TaskPriority.High));
            seed.Tasks.Add(TestData.Task(Guid.NewGuid(), projectId, TaskItemStatus.Todo, TaskPriority.Low));
            seed.Tasks.Add(TestData.Task(Guid.NewGuid(), projectId, TaskItemStatus.Done, TaskPriority.High));
            await seed.SaveChangesAsync();
        }

        await using var context = db.CreateContext();
        var result = await new TaskService(context)
            .GetAllAsync(owner, projectId, TaskItemStatus.Todo, TaskPriority.High);

        var task = Assert.Single(result);
        Assert.Equal(TaskItemStatus.Todo, task.Status);
        Assert.Equal(TaskPriority.High, task.Priority);
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenTaskBelongsToAnotherUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var intruder = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner, intruder);
        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(projectId, owner));
            seed.Tasks.Add(TestData.Task(taskId, projectId));
            await seed.SaveChangesAsync();
        }

        var request = new UpdateTaskRequest("Editada", null,
            TaskItemStatus.Done, TaskPriority.High, null);

        await using var context = db.CreateContext();
        var service = new TaskService(context);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(intruder, projectId, taskId, request));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields_WhenOwnedByUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        await SeedProjectAsync(db, owner, projectId);
        await using (var seed = db.CreateContext())
        {
            seed.Tasks.Add(TestData.Task(taskId, projectId));
            await seed.SaveChangesAsync();
        }

        var request = new UpdateTaskRequest("Concluída", "pronto",
            TaskItemStatus.Done, TaskPriority.High, null);

        await using (var context = db.CreateContext())
            await new TaskService(context).UpdateAsync(owner, projectId, taskId, request);

        await using var check = db.CreateContext();
        var stored = await check.Tasks.FindAsync(taskId);
        Assert.NotNull(stored);
        Assert.Equal("Concluída", stored!.Title);
        Assert.Equal(TaskItemStatus.Done, stored.Status);
    }

    private static async Task SeedProjectAsync(SqliteInMemoryDatabase db, Guid owner, Guid projectId)
    {
        await TestData.SeedUsersAsync(db, owner);
        await using var context = db.CreateContext();
        context.Projects.Add(TestData.Project(projectId, owner));
        await context.SaveChangesAsync();
    }
}
