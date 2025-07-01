using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{

    private readonly PizzaShopContext _context;

    public CustomerRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Customer> list, int totalRecords)> GetPagedCustomerAsync(DateOnly? fromDate, DateOnly? toDate, string searchString, int page, int pagesize, bool isASC, string sortColumn)
    {

        IQueryable<Customer> query = _context.Customers.Where(i => !(bool)i.IsDeleted!).Include(i => i.Orders).ThenInclude(i => i.OrderedItems);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (fromDate != null)
        {
            query = query.Where(i => i.CreatedAt >= fromDate.Value.ToDateTime(TimeOnly.MinValue));

        }

        if (toDate != null)
        {
            query = query.Where(i => i.CreatedAt <= toDate.Value.ToDateTime(TimeOnly.MinValue));

        }

        if (isASC)
        {
            switch (sortColumn)
            {
                case "customer":
                    query = query.OrderBy(u => u.Name);
                    break;

                case "date":
                    query = query.OrderBy(u => u.CreatedAt);
                    break;

                case "ordernumber":
                    query = query.OrderBy(u => u.Orders.Count(cd => cd.CustomerId == u.Id));
                    break;
            }
        }
        else
        {
            switch (sortColumn)
            {
                case "customer":
                    query = query.OrderByDescending(u => u.Name);
                    break;

                case "date":
                    query = query.OrderByDescending(u => u.CreatedAt);
                    break;

                case "ordernumber":
                    query = query.OrderByDescending(u => u.Orders.Count(cd => cd.CustomerId == u.Id));
                    break;
            }
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }

    public async Task<(IEnumerable<Customer>, int)> GetCustomers(string searchString, DateOnly? fromDate, DateOnly? toDate)
    {
        try
        {
            IQueryable<Customer> query = _context.Customers.Include(i => i.Orders);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                query = query.Where(i =>
                    i.Name.ToLower().Contains(searchString) ||
                    i.Email.ToLower().Contains(searchString));
            }

            if (fromDate != null)
            {
                query = query.Where(i => i.CreatedAt >= fromDate.Value.ToDateTime(TimeOnly.MinValue));
            }

            if (toDate != null)
            {
                query = query.Where(i => i.CreatedAt <= toDate.Value.ToDateTime(TimeOnly.MaxValue));
            }

            int totalRecords = await query.CountAsync();
            var customers = await query.ToListAsync();

            return (customers, totalRecords);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return (Enumerable.Empty<Customer>().AsQueryable(), 0);
        }
    }

    public async Task<Customer?> GetCustomerHistory(int customerId)
    {

        var query = await _context.Customers
        .Include(i => i.Orders).ThenInclude(i => i.Invoices).ThenInclude(i => i.Payments)
        .Include(i => i.Orders).ThenInclude(i => i.OrderedItems)
        .FirstOrDefaultAsync(i => i.Id == customerId);

        return query;

    }

    public async Task<Customer?> GetCustomerAsync(int customerId)
    {

        return await _context.Customers
        .Include(i => i.WaitingTokens).ThenInclude(i => i.Section)
        .FirstOrDefaultAsync(i => i.Id == customerId);
    }
    public async Task<Customer?> GetCustomerByEmail(string email)
    {

        return await _context.Customers
        .Include(i => i.WaitingTokens).ThenInclude(i => i.Section)
        .FirstOrDefaultAsync(i => i.Email == email);
    }
    public async Task<Customer?> GetCustomerByEmailOrId(string email , int customerId)
    {

        return await _context.Customers
        .Include(i => i.WaitingTokens).ThenInclude(i => i.Section)
        .FirstOrDefaultAsync(i => i.Email == email || i.Id == customerId);
    }


    public async Task AddCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetCustomerByOrderId(int OrderId)
    {
        return await _context.Customers.Include(i => i.Orders).ThenInclude(i => i.TableOrderMappings).Where(i => i.Orders.FirstOrDefault()!.Id == OrderId).FirstOrDefaultAsync();
    }
    public async Task<Customer?> GetCustomerById(int Id)
    {
        return await _context.Customers.Include(i => i.Orders).ThenInclude(i => i.TableOrderMappings).Where(i => i.Id == Id).FirstOrDefaultAsync();
    }


    public async Task<bool> IsExistsAsync(Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers.AnyAsync(predicate);
    }

    public async Task<int> GetNewCustomerCountAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Customer> query = _context.Customers;

        if (fromDate != null)
        {
            query = query.Where(c => DateOnly.FromDateTime((DateTime)c.CreatedAt) >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(c => DateOnly.FromDateTime((DateTime)c.CreatedAt) <= toDate);
        }

        return await query.CountAsync();
    }

}
