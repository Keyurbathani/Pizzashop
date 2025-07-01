using System.Linq.Expressions;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;

namespace Pizzashop.Repository.Interfaces;

public interface IOrdersRepository
{
    Task<(IEnumerable<Order> list, int totalRecords)> GetPagedOrdersAsync(string searchString, int page, int pagesize, bool isASC, DateOnly? fromDate, DateOnly? toDate, string sortColumn, string status);

    Task<(IEnumerable<Order>, int)> GetOrders(string searchString, string status, DateOnly? fromDate, DateOnly? toDate);

    Task<Order?> GetOrderDetails(int OrderId);

    Task<IEnumerable<Order>> GetOrderByCategory(int categoryId, string status);


    Task<OrderedItem?> GetByIdAsync(int id);
    Task UpdateRange(IEnumerable<OrderedItem> items);

    Task AddOrder(Order order);
    Task UpdateOrder(Order order);
    Task<bool> IsOrderServedAsync(int orderId);
    Task<bool> IsExistsAsync(Expression<Func<Order, bool>> predicate);
    Task<OrderSummaryDTO> GetOrderSummaryAsync(DateOnly? fromDate, DateOnly? toDate);
    Task<(IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData)> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate);

}
