namespace DeliveryAPI.ViewModel
{
    public class DishViewModel
    {
        public string Name
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
        public decimal Price
        {
            get;
            set;
        }
        public string? Image
        {
            get;
            set;
        } 
        public decimal? Rating
        {
            get;
            set;
        }
        public string? Category
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
    public class DishPaginationViewModel
    {
        public List<DishViewModel> Dishes {get;set;}
        public PaginationViewModel Pagination { get;set;}
    }
}
