using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierRepository
{
    Task<Modifier?> GetByIdAsync(int id);
    Task<IEnumerable<Modifier>> GetAllAsync();

    Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedExistingModifiers(string searchStringFor, int page, int pagesize);
    
    Task AddAsync(Modifier modifier);
    Task UpdateAsync(Modifier modifier);
}
