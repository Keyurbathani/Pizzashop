using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IStateRepository
{
    Task<State> GetStateById(int? stateId);
    Task<List<State>> GetStatesByCountryId(int? countryId);
}
