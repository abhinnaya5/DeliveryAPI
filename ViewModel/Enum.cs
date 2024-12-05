using Microsoft.VisualBasic.FileIO;

namespace DeliveryAPI.ViewModel
{
    public enum Gender
    {
        Male,
        Female
    }
    public enum OrderStatus
    {
        InProcess,
        Delivered
    }
    public enum DishCategory
    {
        WOK,
        Pizza,
        Soup,
        Desert,
        Drink
    }
    public enum DishSorting
    {
        NameAsc, 
        NameDesc, 
        PriceAsc, 
        PriceDesc, 
        RatingAsc, 
        RatingDesc
    }

}
