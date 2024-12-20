﻿using Azure.Core;
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
    [Route("api/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private DeliveryDbContext dbContext;
        public DishController(DeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetDish([FromQuery]List<DishCategory> categories, bool vegetarian = false, DishSorting? sorting = null, int page = 1)
        {
            try
            {
                var dishPaginationVMList = new List<DishPaginationViewModel>();
                var dishVMList = new List<DishViewModel>();
                var take = 5;
                var skip = page == 1 ? 0 : take * (page-1);
                var count = 0;

                var dishes = await dbContext.Dishes.ToListAsync();
                if (dishes != null && dishes.Count > 0)
                {     
                    if (categories != null && categories.Count > 0)
                    {
                        dishes = dishes.Where(x => categories.Contains(x.Category)).ToList();
                    }
                    if (vegetarian)
                    {
                        dishes = dishes.Where(x => x.IsVegetarian == vegetarian).ToList();
                    }
                    if (sorting.HasValue)
                    {
                        if (sorting == DishSorting.NameAsc)
                        {
                            dishes = dishes.OrderBy(x => x.Name).ToList();
                        }
                        else if (sorting == DishSorting.NameDesc)
                        {
                            dishes = dishes.OrderByDescending(x => x.Name).ToList();
                        }
                        else if (sorting == DishSorting.PriceAsc)
                        {
                            dishes = dishes.OrderBy(x => x.Price).ToList();
                        }
                        else if (sorting == DishSorting.PriceDesc)
                        {
                            dishes = dishes.OrderByDescending(x => x.Price).ToList();
                        }
                        else if (sorting == DishSorting.RatingAsc)
                        {
                            dishes = dishes.OrderBy(x => x.Rating).ToList();
                        }
                        else if (sorting == DishSorting.RatingDesc)
                        {
                            dishes = dishes.OrderByDescending(x => x.Rating).ToList();
                        }
                    }
                    count = dishes.Count;
                    dishes = dishes.Skip(skip).Take(take).ToList();

                    foreach (var dish in dishes)
                    {
                        //decimal? ratingScore = null;
                        //var rating = await dbContext.Rating.Where(x => x.DishId == dish.DishId).ToListAsync();
                        //if (rating != null && rating.Count > 0)
                        //{
                        //    ratingScore = (decimal)rating.Average(x=>x.Value);
                        //}
                        dishVMList.Add(new DishViewModel()
                        {
                            DishId = dish.DishId,
                            Name = dish.Name,
                            Description = dish.Description,
                            Image = dish.Image,
                            Price = dish.Price,
                            Rating = dish.Rating,
                            Category = dish.Category.ToString(),
                            IsVegetarian = dish.IsVegetarian
                        });
                    }

                    dishPaginationVMList.Add(new DishPaginationViewModel()
                    {
                        Dishes = dishVMList,
                        Pagination = new PaginationViewModel()
                        {
                            Current = page,
                            Size = dishes.Count,
                            Count = count,
                        }
                    });
                }
                return Ok(dishPaginationVMList);
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
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var dish = await dbContext.Dishes.Where(x => x.DishId == id).FirstOrDefaultAsync();
                if (dish == null)
                {
                    return NotFound();
                }
                //decimal? ratingScore = null;
                //var rating = await dbContext.Rating.Where(x => x.DishId == dish.DishId).FirstOrDefaultAsync();
                //if (rating != null)
                //{
                //    ratingScore = rating.Value;
                //}
                var vm = new DishViewModel()
                {
                    DishId = dish.DishId,
                    Name = dish.Name,
                    Description = dish.Description,
                    IsVegetarian = dish.IsVegetarian,
                    Category = dish.Category.ToString(),
                    Image = dish.Image,
                    Price = dish.Price,
                    Rating = dish.Rating,
                };
                return Ok(vm);
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
        [Route("{id:guid}/rating/check")]
        public async Task<IActionResult> CheckRating([FromRoute] Guid id)
        {
            try
            {
                bool check = false;
                var dish = await dbContext.Dishes.Where(x => x.DishId == id).FirstOrDefaultAsync();
                if (dish == null)
                {
                    return NotFound();
                }
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null)
                {
                    var order = await dbContext.Orders.Include(x => x.OrderDetails).Where(x => x.OrderDetails.Any(x => x.DishId == id) && x.UserId.ToString() == user.Value).FirstOrDefaultAsync();
                    if (order != null)
                    {
                        check = true;
                    }
                }
                return Ok(check);
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
        [Route("{id:guid}/rating")]
        public async Task<IActionResult> SetRating([FromRoute] Guid id, int? ratingScore)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user != null && ratingScore.HasValue)
                {
                    var dish = await dbContext.Dishes.Where(x => x.DishId == id).FirstOrDefaultAsync();
                    if (dish != null)
                    {
                        //var rating = await dbContext.Rating.Where(x => x.DishId == id && x.UserId.ToString() == user.Value).FirstOrDefaultAsync();
                        //if (rating != null)
                        //{
                        //    rating.Value = ratingScore.Value;
                        //}
                        //else
                        //{
                        //    var newRating = new Rating()
                        //    {
                        //        UserId = Guid.Parse(user.Value),
                        //        DishId = id,
                        //        RatingId = Guid.NewGuid(),
                        //        Value = ratingScore.Value,
                        //    };
                        //    await dbContext.Rating.AddAsync(newRating);
                        //    dish.Rating = dish.Rating.HasValue ? (dish.Rating + ratingScore) / 2 : ratingScore.Value;
                        //}

                        var newRating = new Rating()
                        {
                            UserId = Guid.Parse(user.Value),
                            DishId = id,
                            RatingId = Guid.NewGuid(),
                            Value = ratingScore.Value,
                        };
                        await dbContext.Rating.AddAsync(newRating);
                        dish.Rating = dish.Rating.HasValue ? (dish.Rating + ratingScore) / 2 : ratingScore.Value;
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
    }
}
