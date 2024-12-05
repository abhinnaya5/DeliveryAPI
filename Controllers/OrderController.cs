using DeliveryAPI.Data;
using DeliveryAPI.Models.Domain;
using DeliveryAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliveryAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private DeliveryDbContext dbContext;
        public OrderController(DeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrder()
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var orderVMList = new List<OrderPageViewModel>();
                    var orders = await dbContext.Orders.Where(x => x.UserId.ToString() == user.Value).ToListAsync();
                    if (orders != null && orders.Count > 0)
                    {
                        foreach (var order in orders)
                        {
                            orderVMList.Add(new OrderPageViewModel()
                            {
                                Id = order.OrderId,
                                Status = order.OrderStatus.ToString(),
                                DeliveryTime = order.DeliveryTime,
                                OrderTime = order.OrderTime,
                                Price = order.Price,
                            });
                        }
                    }
                    return Ok(orderVMList);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var dishVMList = new List<CartViewModel>();
            var order = await dbContext.Orders.Include(x=>x.OrderDetails).Where(x => x.OrderId == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }
            if (order.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var dish = await dbContext.Dishes.Where(x => x.DishId == detail.DishId).FirstOrDefaultAsync();
                    if (dish != null)
                    {
                        var cart = await dbContext.Cart.Where(x => x.DishId == dish.DishId && x.UserId == order.UserId).FirstOrDefaultAsync();
                        dishVMList.Add(new CartViewModel()
                        {
                            Id = dish.DishId,
                            Name = dish.Name,
                            Amount = cart != null ? cart.Qty : 0,
                            Image = dish.Image,
                            Price = dish.Price,
                            TotalPrice = cart != null ? dish.Price * cart.Qty : 0,
                        });
                    }
                }
            }

            var vm = new OrderViewModel()
            {
                Id = order.OrderId,
                Address = order.Address,
                DeliveryTime = order.DeliveryTime,
                Price = order.Price,
                OrderTime = order.OrderTime,
                Status = order.OrderStatus.ToString(),
                Dishes = dishVMList
            };
            return Ok(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel createOrderRequest)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var carts = await dbContext.Cart.Where(x => x.UserId.ToString() == user.Value).ToListAsync();
                    if (carts != null && carts.Count > 0)
                    {
                        decimal totalPrice = 0;
                        var orderDetails = new List<OrderDetail>();
                        var orderId = Guid.NewGuid();
                        foreach (var cart in carts)
                        {
                            var dish = await dbContext.Dishes.Where(x => x.DishId == cart.DishId).FirstOrDefaultAsync();
                            if (dish != null)
                            {
                                totalPrice += (dish.Price * cart.Qty);
                                orderDetails.Add(new OrderDetail()
                                {
                                    OrderDetailId = Guid.NewGuid(),
                                    DishId = cart.DishId,
                                    OrderId = orderId,
                                });
                            }
                        }
                        var newOrder = new Order()
                        {
                            OrderId = orderId,
                            UserId = Guid.Parse(user.Value),
                            Address = createOrderRequest.Address,
                            DeliveryTime = createOrderRequest.DeliveryTime,
                            OrderTime = DateTime.Now,
                            Price = totalPrice,
                            OrderStatus = OrderStatus.InProcess,
                            OrderDetails = orderDetails,
                        };
                        await dbContext.Orders.AddAsync(newOrder);
                        await dbContext.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("{id:guid}/status")]
        public async Task<IActionResult> ConfirmOrder([FromRoute] Guid id)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var order = await dbContext.Orders.Include(x => x.OrderDetails).Where(x => x.OrderId == id).FirstOrDefaultAsync();
                    if (order != null)
                    {
                        order.OrderStatus = OrderStatus.Delivered;
                        await dbContext.SaveChangesAsync();

                        //delete cart
                        if (order.OrderDetails != null && order.OrderDetails.Count > 0)
                        {
                            foreach (var detail in order.OrderDetails)
                            {
                                var cart = await dbContext.Cart.Where(x => x.DishId == detail.DishId && x.UserId.ToString() == user.Value).FirstOrDefaultAsync();
                                if (cart != null)
                                {
                                    dbContext.Remove(cart);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel()
                {
                    Status = "Error",
                    Message = e.Message,
                });
            }
        }
    }
}
