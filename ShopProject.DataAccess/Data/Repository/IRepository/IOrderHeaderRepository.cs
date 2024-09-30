using ShopProject.Models;

namespace ShopProject.DataAccess.Data.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
    void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
    void UpdatePaymentID(int id, string sessionId, string paymentIntentId);
}