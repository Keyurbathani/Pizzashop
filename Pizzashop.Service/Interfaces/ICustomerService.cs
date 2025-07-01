using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ICustomerService
{
    Task<Response<PagedResult<CustomerListVM>>> GetPagedCustomerAsync(DateOnly? fromDate, DateOnly? toDate , string searchString, int page, int pagesize, bool isASC , string sortColumn);

    Task<Response<CustomerHistoryVM>> GetCustomerHistory(int customerId);

    Task<Response<MemoryStream>> ExportCustomer(string searchQuery, string account, DateOnly? fromDate, DateOnly? toDate);
}
