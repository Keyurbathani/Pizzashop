using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ITaxService
{
    Task<Response<PagedResult<TaxListVM>>> GetPagedItemsAsync(string searchString, int page, int pagesize, bool isASC);

    Task<TaxVM?> GetTaxByIdAsync(int id);

    Task<Response<bool>> AddTax(TaxVM model, HttpRequest request);

    Task<Response<bool>> EditTax(TaxVM model, HttpRequest request);

    Task<bool> DeleteTax(int id, HttpRequest request);
    
}
