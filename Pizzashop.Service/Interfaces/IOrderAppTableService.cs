using Pizzashop.Entity;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IOrderAppTableService
{
    Task<Response<IEnumerable<OrderAppSectionListVM>>> GetTablesForOrderApp();
    
}
