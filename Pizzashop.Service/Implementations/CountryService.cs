using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementations;

public class CountryService : ICountryService
{

     private readonly ICountryRepository _countryRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IRoleRepository _roleRepository;

    public CountryService(ICountryRepository countryRepository, IStateRepository stateRepository, ICityRepository cityRepository, IRoleRepository roleRepository)
    {
        
        _cityRepository = cityRepository;
        _countryRepository = countryRepository;
        _roleRepository = roleRepository;
        _stateRepository = stateRepository;
    }
       public async Task<List<Country>> GetAllCountries()
    {
        return await _countryRepository.GetAllCountries();
    }
       public async Task<List<Role>> GetRoles()
    {
        return await _roleRepository.GetAllRoles();
    }

    public async Task<List<State>> GetStatesByCountryId(int? countryId)
    {
        return await _stateRepository.GetStatesByCountryId(countryId);
    }

    public async Task<List<City>> GetCitiesByStateId(int? stateId)
    {
        return await _cityRepository.GetCitiesByStateId(stateId);
    }
    
}
