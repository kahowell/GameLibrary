using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAll();
    Task Add(Company company);
    Task<int> Count();
    Task DeleteAll();
    Task<Company?> FindExistingAsync(Company company);
}
