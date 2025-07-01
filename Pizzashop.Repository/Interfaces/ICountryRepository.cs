using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICountryRepository
{
    Task<Country> GetCountryById(int countryId);

    Task<List<Country>> GetAllCountries();
}
