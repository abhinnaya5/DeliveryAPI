namespace DeliveryAPI.ViewModel
{
    public class CartViewModel
    {
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
        public decimal TotalPrice
        {
            get;
            set;
        } 
        public decimal Amount
        {
            get;
            set;
        }
        public string? Image
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
}
