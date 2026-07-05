using TaskManager.Api.Common;
using TaskManager.Api.Dtos;
using TaskManager.Api.Services;

namespace TaskManager.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsProjectForOwner()
    {
        using var db = new SqliteInMemoryDatabase();
        var ownerId = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, ownerId);

        ProjectResponse response;
        await using (var context = db.CreateContext())
            response = await new ProjectService(context)
                .CreateAsync(ownerId, new CreateProjectRequest("Portfolio", "desc"));

        await using (var check = db.CreateContext())
        {
            var stored = Assert.Single(check.Projects);
            Assert.Equal(response.Id, stored.Id);
            Assert.Equal("Portfolio", stored.Name);
            Assert.Equal(ownerId, stored.OwnerId);
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyProjectsOwnedByTheUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var other = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner, other);

        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(Guid.NewGuid(), owner, "A"));
            seed.Projects.Add(TestData.Project(Guid.NewGuid(), owner, "B"));
            seed.Projects.Add(TestData.Project(Guid.NewGuid(), other, "C"));
            await seed.SaveChangesAsync();
        }

        await using var context = db.CreateContext();
        var result = await new ProjectService(context).GetAllAsync(owner);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.Name == "C");
    }

    [Fact]
    public async Task GetByIdAsync_Throws_WhenProjectBelongsToAnotherUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var intruder = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner, intruder);

        var projectId = Guid.NewGuid();
        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(projectId, owner));
            await seed.SaveChangesAsync();
        }

        await using var context = db.CreateContext();
        var service = new ProjectService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(intruder, projectId));
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenProjectBelongsToAnotherUser()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        var intruder = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner, intruder);

        var projectId = Guid.NewGuid();
        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(projectId, owner));
            await seed.SaveChangesAsync();
        }

        await using var context = db.CreateContext();
        var service = new ProjectService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(intruder, projectId));

        await using var check = db.CreateContext();
        Assert.Single(check.Projects);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProject_AndCascadesTasks()
    {
        using var db = new SqliteInMemoryDatabase();
        var owner = Guid.NewGuid();
        await TestData.SeedUsersAsync(db, owner);

        var projectId = Guid.NewGuid();
        await using (var seed = db.CreateContext())
        {
            seed.Projects.Add(TestData.Project(projectId, owner));
            seed.Tasks.Add(TestData.Task(Guid.NewGuid(), projectId));
            seed.Tasks.Add(TestData.Task(Guid.NewGuid(), projectId));
            await seed.SaveChangesAsync();
        }

        await using (var context = db.CreateContext())
            await new ProjectService(context).DeleteAsync(owner, projectId);

        await using (var check = db.CreateContext())
        {
            Assert.Empty(check.Projects);
            Assert.Empty(check.Tasks);
        }
    }
}
