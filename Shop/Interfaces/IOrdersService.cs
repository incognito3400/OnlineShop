using Exam2.Backend.Entities;

namespace Shop.Interfaces
{
    public interface IOrdersService
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        IEnumerable<Order> GetOrdersByUserId(int userId);
        void CreateOrder(Order order);
        void UpdateOrderStatus(int orderId, string status);
        void DeleteOrder(int id);
    }
}
