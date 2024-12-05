using DeliveryAPI.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.Domain
{
    public class Order
    {
        [Key]
        public Guid OrderId
        {
            get;
            set;
        }
        public Guid UserId
        {
            get;
            set;
        }
        public DateTime DeliveryTime
        {
            get;
            set;
        }
        public DateTime OrderTime
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public OrderStatus OrderStatus
        {
            get;
            set;
        }
        public List<OrderDetail> OrderDetails
        {
            get;
            set;
        }
    }
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailId
        {
            get;
            set;
        }
        public Guid OrderId
        {
            get;
            set;
        }
        public Guid DishId
        {
            get;
            set;
        } 
        public Dish Dish
        {
            get;
            set;
        }
    }
}
