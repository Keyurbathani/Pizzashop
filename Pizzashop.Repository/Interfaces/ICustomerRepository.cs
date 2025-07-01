using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<(IEnumerable<Customer> list, int totalRecords)> GetPagedCustomerAsync(DateOnly? fromDate, DateOnly? toDate , string searchString, int page, int pagesize, bool isASC , string sortColumn);

    Task<Customer?> GetCustomerHistory(int customerId);

    Task<(IEnumerable<Customer>, int)> GetCustomers(string searchString, DateOnly? fromDate, DateOnly? toDate);
    
    Task<Customer?> GetCustomerAsync(int customerId);

    Task<Customer?> GetCustomerByEmail(string email);
    Task<Customer?> GetCustomerByEmailOrId(string email , int customerId);
    Task AddCustomer(Customer customer);
    Task UpdateCustomer(Customer customer);
    Task<Customer?> GetCustomerByOrderId(int OrderId);
    Task<Customer?> GetCustomerById(int Id);
    Task<bool> IsExistsAsync(Expression<Func<Customer, bool>> predicate);
    Task<int> GetNewCustomerCountAsync(DateOnly? fromDate, DateOnly? toDate);

    
}
