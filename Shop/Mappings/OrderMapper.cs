using Exam2.Backend.Entities;
using Shop.DTOs;

namespace Shop.Mappings
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ItemsCount = order.Items?.Count ?? 0
            };
        }

        public static OrderDetailDto ToDetailDto(this Order order)
        {
            return new OrderDetailDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ItemsCount = order.Items?.Count ?? 0,
                Items = order.Items?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList() ?? new List<OrderItemDto>()
            };
        }

        public static CreateOrderResponse ToCreateResponse(this Order order)
        {
             return new CreateOrderResponse
             {
                 Id = order.Id,
                 Status = order.Status,
                 TotalAmount = order.TotalAmount
             };
        }
    }
}
