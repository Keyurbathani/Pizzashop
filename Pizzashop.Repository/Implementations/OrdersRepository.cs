using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrdersRepository : IOrdersRepository
{

    private readonly PizzaShopContext _context;

    public OrdersRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Order> list, int totalRecords)> GetPagedOrdersAsync(string searchString, int page, int pagesize, bool isASC, DateOnly? fromDate, DateOnly? toDate, string sortColumn, string status)
    {

        IQueryable<Order> query = _context.Orders.Include(i => i.Customer).Include(i => i.Feedbacks).Include(i => i.Invoices).ThenInclude(i => i.Payments);


        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Customer.Name.ToLower().Contains(searchString) || i.Id.ToString().Contains(searchString));
        }

        if (!string.IsNullOrEmpty(status))
        {
            // status = status.ToLower().Trim();
            query = query.Where(i => i.OrderStatus!.Contains(status));
        }



        if (fromDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) <= toDate);
        }

        if (isASC)
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderBy(i => i.Customer.Name),
                "totalamount" => query.OrderBy(i => i.PaidAmount),
                "date" => query.OrderBy(i => DateOnly.FromDateTime(i.CreatedAt)),
                _ => query.OrderBy(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };

        }
        else
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderByDescending(i => i.Customer.Name),
                "totalamount" => query.OrderByDescending(i => i.PaidAmount),
                "date" => query.OrderByDescending(i => DateOnly.FromDateTime(i.CreatedAt)),
                _ => query.OrderByDescending(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };

        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }
    public async Task<(IEnumerable<Order>, int)> GetOrders(string searchString, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        try
        {
            var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Feedbacks)
            .Include(o => o.Invoices)
            .ThenInclude(o => o.Payments)
            .AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(o => o.Id.ToString().Contains(searchString) ||
                                         o.Customer.Name.ToLower().Contains(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus!.ToLower().Trim() == status.ToLower().Trim());
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) >= fromDate && DateOnly.FromDateTime(i.CreatedAt) <= toDate);
            }

            int totalRecords = await query.CountAsync();
            var orders = await query.ToListAsync();

            return (orders, totalRecords);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return (Enumerable.Empty<Order>().AsQueryable(), 0);
        }

    }
    public async Task<Order?> GetOrderDetails(int OrderId)
    {
        var query = await _context.Orders
        .Include(i => i.Customer).ThenInclude(i => i.WaitingTokens)
        .Include(i => i.TableOrderMappings).ThenInclude(i => i.Table).ThenInclude(i => i.Section)
        .Include(i => i.OrderTaxMappings).ThenInclude(i => i.Tax)
        .Include(i => i.OrderedItems).ThenInclude(i => i.MenuItem)
        .Include(i => i.OrderedItems).ThenInclude(i => i.OrderedItemModifierMappings).ThenInclude(i => i.Modifier)
        .Include(i => i.Invoices).ThenInclude(i => i.Payments)
        .Include(i => i.Feedbacks)
        .FirstOrDefaultAsync(i => i.Id == OrderId);

        return query;
    }
    public async Task<IEnumerable<Order>> GetOrderByCategory(int categoryId, string status)
    {

        IQueryable<Order> query = _context.Orders
        .Include(i => i.OrderedItems).ThenInclude(i => i.MenuItem).ThenInclude(i => i.Category)
        .Include(i => i.TableOrderMappings).ThenInclude(i => i.Table).ThenInclude(i => i.Section)
        .Include(i => i.OrderedItems).ThenInclude(i => i.OrderedItemModifierMappings).ThenInclude(i => i.Modifier).Where(i => i.OrderStatus != "Completed" && i.OrderStatus != "Cancelled");

        return await query.ToListAsync();

    }

  
    public async Task<OrderedItem?> GetByIdAsync(int id)
    {
        return await _context.OrderedItems.SingleOrDefaultAsync(i => i.Id == id);
    }
    public async Task UpdateRange(IEnumerable<OrderedItem> items)
    {
        _context.OrderedItems.UpdateRange(items);
        await _context.SaveChangesAsync();
    }
    public async Task AddOrder(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> IsOrderServedAsync(int orderId)
    {
        return (await _context.Orders.Where(o => o.Id == orderId).Select(o => o.OrderStatus).SingleOrDefaultAsync() ?? "").Equals("Served");
    }
    public async Task<bool> IsExistsAsync(Expression<Func<Order, bool>> predicate)
    {
        return await _context.Orders.AnyAsync(predicate);
    }


    public async Task<OrderSummaryDTO> GetOrderSummaryAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.OrderBy(o => o.CreatedAt);
 
        if (fromDate != null)
        {
            query = query.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= fromDate);
        }
 
        if (toDate != null)
        {
            query = query.Where(o => DateOnly.FromDateTime(o.CreatedAt) <= toDate);
        }
 
        OrderSummaryDTO orderSummaryDTO = new()
        {
            TotalOrders = await query.CountAsync(),
            TotalSales = await query.SumAsync(o => o.TotalAmount.GetValueOrDefault()),
            AvgWaitingTime = TimeSpan.FromTicks((long)(await query
                .Where(o => o.ModifiedAt.HasValue)
                .Select(o => (o.ModifiedAt.GetValueOrDefault() - o.CreatedAt).Ticks).ToListAsync()).DefaultIfEmpty(0).Average())
        };
 
        orderSummaryDTO.AvgOrderValue = orderSummaryDTO.TotalSales / (orderSummaryDTO.TotalOrders > 0 ? orderSummaryDTO.TotalOrders : 1);
 
        return orderSummaryDTO;
    }
 
    public async Task<(IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData)> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Order> query = _context.Orders.OrderBy(o => o.CreatedAt);
       
 
        if (fromDate != null)
        {
            query = query.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= fromDate);
        }
 
        if (toDate != null)
        {
            query = query.Where(o => DateOnly.FromDateTime(o.CreatedAt) <= toDate);
        }
 
 
        DateTime firstDate = fromDate?.ToDateTime(TimeOnly.MinValue) ?? (await query.FirstOrDefaultAsync())?.CreatedAt ?? DateTime.MinValue;
        DateTime lastDate = toDate?.ToDateTime(TimeOnly.MaxValue) ?? (await query.LastOrDefaultAsync())?.CreatedAt ?? DateTime.MinValue;
 
        int dataPoints;
 
 
        if (firstDate.AddDays(1) >= lastDate){
            dataPoints = 24;
        }
        else if(firstDate.AddDays(31) >= lastDate){
            dataPoints = lastDate.Subtract(firstDate).Days +1;
        }
        else{
            dataPoints = 5;
        }
 
        TimeSpan interval = (lastDate - firstDate) / dataPoints;
        List<GraphDataDTO> revenueData = new();
        List<GraphDataDTO> customerGrowthData = new();
 
        for (int i = 0; i < dataPoints; i++)
        {
            DateTime intervalStart = firstDate.Add(interval * i);
            DateTime intervalEnd = intervalStart.Add(interval);
            IQueryable<Order> segmentQuery = query.Where(o => o.CreatedAt >= intervalStart && o.CreatedAt < intervalEnd);
 
            decimal revenue = await segmentQuery.SumAsync(o => o.TotalAmount.GetValueOrDefault());
 
            decimal customerCount = await segmentQuery.GroupBy(o => o.Customer.Id).CountAsync();
 
            string date;
 
            if(firstDate.AddDays(1) >= lastDate){
                date = intervalStart.ToString("hh:mm") ;
            }
            else if(firstDate.AddDays(31) >= lastDate){
                date = intervalStart.ToString("dd MMM yyyy");
            }
            else{
                date = intervalStart.ToString("dd MMM yyyy") + " - " + intervalEnd.ToString("dd MMM yyyy");
            }
 
            revenueData.Add(new GraphDataDTO
            {
                Date = date,
                Value = revenue
            });
 
            customerGrowthData.Add(new GraphDataDTO{
                Date = date,
                Value = customerCount
            });
 
        }
 
        return (revenueData, customerGrowthData);
 
    }
}
