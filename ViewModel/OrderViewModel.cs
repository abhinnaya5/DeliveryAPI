using DeliveryAPI.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.ViewModel
{
    public class OrderViewModel
    {
        public List<CartViewModel> Dishes
        {
            get;
            set;
        }
        public string Address
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
        public string Status
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public Guid Id
        {
            get;
            set;
        }
    }

    public class OrderPageViewModel
    {
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

        public string Status
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public Guid Id
        {
            get;
            set;
        }
    }

    public class CreateOrderViewModel
    {
        public DateTime DeliveryTime
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
    }

    public class OrderDetailViewModel
    {
        [Key]
        public Guid OrderDetailId
        {
            get;
            set;
        }
        public Guid DishId
        {
            get;
            set;
        }
    }
}
