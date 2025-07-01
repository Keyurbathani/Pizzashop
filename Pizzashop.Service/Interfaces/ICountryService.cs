using Pizzashop.Entity.Data;

namespace Pizzashop.Service.Interfaces;

public interface ICountryService
{
    Task<List<Country>> GetAllCountries();
    Task<List<Role>> GetRoles();
    Task<List<State>> GetStatesByCountryId(int? countryId);
    Task<List<City>> GetCitiesByStateId(int? stateId);

}
