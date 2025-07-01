using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class StateRepository : IStateRepository
{
     private readonly PizzaShopContext _context;

    public StateRepository(PizzaShopContext context)
    {
        _context = context;
    }

      public async Task<State> GetStateById(int? stateId)
    {
        return await _context.States.FirstOrDefaultAsync(c => c.Id == stateId);
    }

    public async Task<List<State>> GetStatesByCountryId(int? countryId)
    {
        return await _context.States.Where(c => c.CountryId == countryId).OrderBy(c => c.Name).ToListAsync();
    }


}
