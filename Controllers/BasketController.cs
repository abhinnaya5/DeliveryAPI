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
    [Route("api/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private DeliveryDbContext dbContext;
        public BasketController(DeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var cartVMList = new List<CartViewModel>();
                    var carts = await dbContext.Cart.Where(x => x.UserId.ToString() == user.Value).ToListAsync();
                    if (carts != null && carts.Count > 0)
                    {
                        foreach(var cart in carts)
                        {
                            var dish = await dbContext.Dishes.Where(x => x.DishId == cart.DishId).FirstOrDefaultAsync();
                            if (dish != null)
                            {
                                cartVMList.Add(new CartViewModel()
                                {
                                    Id = dish.DishId,
                                    Name = dish.Name,
                                    Amount = cart.Qty,
                                    Image = dish.Image,
                                    Price = dish.Price,
                                    TotalPrice = dish.Price * cart.Qty,
                                });
                            }
                        }
                    }
                    return Ok(cartVMList);
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
        [Route("dish/{dishId:guid}")]
        public async Task<IActionResult> AddOrUpdateCart([FromRoute] Guid dishId)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var dish = await dbContext.Dishes.Where(x => x.DishId == dishId).FirstOrDefaultAsync();
                    if (dish != null)
                    {
                        var cart = await dbContext.Cart.Where(x => x.DishId == dishId && x.UserId.ToString() == user.Value).FirstOrDefaultAsync();
                        if (cart != null)
                        {
                            cart.DishId = dishId;
                            cart.UserId = Guid.Parse(user.Value);
                            cart.Qty += 1;
                            await dbContext.SaveChangesAsync();
                            return Ok();
                        }
                        else
                        {
                            var newCart = new Cart()
                            {
                                CartId = Guid.NewGuid(),
                                UserId = Guid.Parse(user.Value),
                                DishId = dishId,
                                Qty = 1,
                            };
                            await dbContext.Cart.AddAsync(newCart);
                            await dbContext.SaveChangesAsync();
                            return Ok();
                        }
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

        [HttpDelete]
        [Authorize]
        [Route("dish/{dishId:guid}")]
        public async Task<IActionResult> DeleteCart(Guid dishId, bool increase = false)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var cart = await dbContext.Cart.Where(x => x.DishId == dishId && x.UserId.ToString() == user.Value).FirstOrDefaultAsync();
                    if (cart != null)
                    {
                        if (increase)
                        {
                            cart.Qty -= 1;
                        }
                        else
                        {
                            dbContext.Remove(cart);
                        }
                        dbContext.SaveChangesAsync();
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
