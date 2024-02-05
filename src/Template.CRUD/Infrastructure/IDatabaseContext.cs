using Template.Api.Features.Users.Models;

namespace Template.Api.Infrastructure;

public interface IDatabaseContext
{
    DbSet<UserModel> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
