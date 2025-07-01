using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IWaitingTokensRepository
{
    Task<IEnumerable<WaitingToken>> GetAllTokensBySection(int sectionId);
    Task<WaitingToken?> GetByIdAsync(int id);
    Task<WaitingToken?> GetByCustomerId(int customerId);
    Task AddToken( WaitingToken token);
    Task UpdateToken( WaitingToken token);
    Task DeleteToken( WaitingToken token);
    Task<int> GetWaitingCustomerCountAsync();
    Task<bool> IsExistsAsync(Expression<Func<WaitingToken, bool>> predicate);
    
}
