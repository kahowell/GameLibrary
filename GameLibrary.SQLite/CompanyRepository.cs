using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.SQLite;

public class CompanyRepository(IDbContextFactory<LibraryContext> libraryContextFactory) : ICompanyRepository
{
    public async Task<IEnumerable<Company>> GetAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return libraryContext.Companies.AsEnumerable();
    }

    public async Task Add(Company company)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        libraryContext.Companies.Update(company);
        await libraryContext.SaveChangesAsync();
    }

    public async Task<int> Count()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Companies.CountAsync();
    }

    public async Task DeleteAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await libraryContext.Companies.ExecuteDeleteAsync();
        await libraryContext.SaveChangesAsync();
    }

    public async Task<Company?> FindExistingAsync(Company company)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Companies
            .Where(c =>
                c.Id == company.Id || (c.IgdbId != null && company.IgdbId != null && c.IgdbId == company.IgdbId)
            )
            .FirstOrDefaultAsync();
    }
}
