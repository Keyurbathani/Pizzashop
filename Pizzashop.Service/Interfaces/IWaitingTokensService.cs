using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IWaitingTokensService
{
    Task<Response<IEnumerable<WaitingTokenList>>> GetAllTokensBySection(int sectionId);
    Task<Response<WaitingTokenList>> GetCustomerAsync(int customerId);
    Task<Response<WaitingTokenList>> GetCustomerByEmail(string email);
    Task<WaitingTokenList?> GetTokenByIdAsync(int id);
    Task<Response<bool>> CreateToken(WaitingTokenList model);
    Task<Response<bool>> EditToken(WaitingTokenList model);
    Task<Response<bool>> DeleteToken(int id);
    Task<Response<Order>> AssignTableAddOrder(int customerId, IEnumerable<int> ids,  int tokenId);
}
