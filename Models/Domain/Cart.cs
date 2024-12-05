using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.Domain
{
    public class Cart
    {
        public Guid CartId
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
        public int Qty
        {
            get;
            set;
        }
    }
}
