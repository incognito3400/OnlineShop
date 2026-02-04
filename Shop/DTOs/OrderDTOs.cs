using Exam2.Backend.Entities;

namespace Shop.DTOs
{
    // Responses
    public class OrderDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDetailDto : OrderDto
    {
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class GetOrdersResponse
    {
        public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }

    public class GetOrderByIdResponse
    {
        public OrderDetailDto Order { get; set; } = new OrderDetailDto();
    }

    public class CreateOrderResponse
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // Requests
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderRequest
    {
        public int? UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
