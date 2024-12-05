using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.Domain
{
    public class Rating
    {
        [Key]
        public Guid RatingId
        {
            get;
            set;
        }
        public Guid DishId
        {
            get;
            set;
        }
        public Guid UserId
        {
            get;
            set;
        }
        public decimal Value
        {
            get;
            set;
        }
    }
}
