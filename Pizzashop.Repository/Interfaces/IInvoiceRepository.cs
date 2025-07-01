using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IInvoiceRepository
{
    Task AddInvoice(Invoice invoice);
    Task<Invoice?> GetByOrderId(int orderId);
}
