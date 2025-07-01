using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ISectionService
{
    Task<Response<IEnumerable<SectionVM>>> GetAllSectionAsync();
    Task<Response<IEnumerable<TableVM>>> GetAllTableVM(int sectionId);

    Task<Response<PagedResult<TableListVM>>> GetPagedItemsAsync(int SectionId, string searchString, int page, int pagesize, bool isASC);

    Task<Response<SectionVM?>> AddSection(SectionVM model, HttpRequest request);

    Task<Response<SectionVM?>> EditSection(SectionVM model, HttpRequest request);

    Task<Response<bool>> DeleteSection(int id, HttpRequest request);

    Task<TableVM?> GetTableByIdAsync(int id);

    Task<Response<TableVM>> AddTables(TableVM model, HttpRequest request);

    Task<Response<TableVM>> EditTables(TableVM model, HttpRequest request);

    Task<Response<bool>> DeleteTable(int id, HttpRequest request);

    Task<Response<bool>> DeleteManyTableAsync(IEnumerable<int> ids, HttpRequest request);
}
