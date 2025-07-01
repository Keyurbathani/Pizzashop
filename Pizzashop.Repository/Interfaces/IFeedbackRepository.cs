using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IFeedbackRepository
{
    Task AddFeedback(Feedback feedback);
    Task UpdateFeedback(Feedback feedback);
    Task<Feedback?> GetFeedbackByOrderId(int OrderId);
}
