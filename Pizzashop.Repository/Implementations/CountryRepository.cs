using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CountryRepository : ICountryRepository
{
     private readonly PizzaShopContext _context;

    public CountryRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<Country> GetCountryById(int countryId)
    {
        return await _context.Countries.FirstOrDefaultAsync(c => c.Id == countryId);
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await _context.Countries.OrderBy(c => c.Name).ToListAsync();
    }
   
}
