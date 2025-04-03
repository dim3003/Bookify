using Bookify.Domain.Users;

namespace Bookify.Domain.Abstractions;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Add(User user);
}
