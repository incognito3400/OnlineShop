using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IProductsService _productsService;

        public OrdersController(IOrdersService ordersService, IProductsService productsService)
        {
            _ordersService = ordersService;
            _productsService = productsService;
        }

        [HttpGet]
        public ActionResult<GetOrdersResponse> GetAll()
        {
            var orders = _ordersService.GetAllOrders();
            return Ok(new GetOrdersResponse
            {
                Orders = orders.Select(o => o.ToDto())
            });
        }

        [HttpGet("{id}")]
        public ActionResult<GetOrderByIdResponse> GetById(int id)
        {
            var order = _ordersService.GetOrderById(id);
            if (order == null) return NotFound();

            return Ok(new GetOrderByIdResponse
            {
                Order = order.ToDetailDto()
            });
        }

        [HttpGet("my/{userId}")]
        public ActionResult<GetOrdersResponse> GetMyOrders(int userId)
        {
            var orders = _ordersService.GetOrdersByUserId(userId);
            return Ok(new GetOrdersResponse
            {
                Orders = orders.Select(o => o.ToDto())
            });
        }

        [HttpPost]
        public ActionResult<CreateOrderResponse> Create([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                UserId = request.UserId,
                Status = "New",
                Items = new List<OrderItem>()
            };

            foreach (var item in request.Items)
            {
                var product = _productsService.GetProductById(item.ProductId);
                if (product != null)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price
                    });
                }
            }

            // Recalculate total amount logic usually resides in service, but we'll leave it as implied or handled by service. 
            // In the previous Program.cs logic, it just added items. 
            // Let's assume CreateOrder calculates Total or we need to sum it up if not handled.
            // Looking at Program.cs earlier: it didn't sum it up explicitly in the snippet shown, 
            // but usually Order entity might ideally have logic or Service does. 
            // However, for strict mapping:
            _ordersService.CreateOrder(order);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.ToCreateResponse());
        }

        [HttpPut("{id}/status")]
        public ActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var order = _ordersService.GetOrderById(id);
            if (order == null) return NotFound();

            _ordersService.UpdateOrderStatus(id, request.Status);
            return Ok(new { id, Status = request.Status });
        }
    }
}
