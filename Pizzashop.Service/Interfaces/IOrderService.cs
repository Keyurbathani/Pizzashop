using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IOrderService
{
    Task<Response<PagedResult<OrderListVM>>> GetPagedOrdersAsync(DateOnly? fromDate, DateOnly? toDate,string searchString, int page, int pagesize, bool isASC,  string sortColumn , string status);

    Task<Response<MemoryStream>> ExportOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate);

    Task<Response<IEnumerable<KotOrderVM>>> GetOrderByCategory(int categoryId,string status);

    Task<Response<OrderDetailsVM?>> GetOrderByIdAsync(int id);
    Task<Response<bool>> UpdateOrderItemStatusAsync(int orderId, string status, List<OrderItem> updatedOrderItems);

}
