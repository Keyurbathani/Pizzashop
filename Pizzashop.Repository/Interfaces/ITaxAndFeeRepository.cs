using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITaxAndFeeRepository
{
    Task<TaxAndFee?> GetByIdAsync(int id);
    Task<IEnumerable<TaxAndFee>> GetEnabledAsync();
}
