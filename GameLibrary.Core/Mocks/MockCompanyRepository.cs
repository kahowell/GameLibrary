using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockCompanyRepository: ICompanyRepository
{
    private readonly IList<Company> _companies =
    [
        new() { Id = Guid.NewGuid(), Name = "Acme, Inc", StartDate = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Name = "Gamma Co.", StartDate = DateTimeOffset.UtcNow }
    ];

    public Task<IEnumerable<Company>> GetAll()
    {
        return Task.FromResult(_companies.AsEnumerable());
    }

    public Task Add(Company company)
    {
        return Task.CompletedTask;
    }

    public Task<int> Count()
    {
        return Task.FromResult(_companies.Count);
    }

    public Task DeleteAll()
    {
        return Task.CompletedTask;
    }

    public Task<Company?> FindExistingAsync(Company company) => Task.FromResult(null as Company);
}
