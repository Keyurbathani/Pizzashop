using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IPaymentRepository
{
    Task AddPayment(Payment payment);
    Task<Payment?> GetByInvoiceId(int invoiceId);
    Task UpdatePayment(Payment payment);
}
