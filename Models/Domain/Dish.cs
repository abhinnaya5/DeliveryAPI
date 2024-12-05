using DeliveryAPI.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.Domain
{
    public class Dish
    {
        [Key]
        public Guid DishId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }

        public bool IsVegetarian
        {
            get;
            set;
        }
        public string? Image
        {
            get;
            set;
        }
        public DishCategory Category
        {
            get;
            set;
        }
        public decimal? Rating
        {
            get;
            set;
        }
    }
}
