using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICityRepository
{
    Task<City> GetCityById(int? cityId);
    Task<List<City>> GetCitiesByStateId(int? stateId);
 
}
