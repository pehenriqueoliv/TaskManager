using Microsoft.AspNetCore.Identity;

namespace TaskManager.Api.Entities;

public class AppUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAt { get; set; }
}
