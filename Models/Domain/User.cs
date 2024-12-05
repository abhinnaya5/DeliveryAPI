using DeliveryAPI.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.Domain
{
    public class User
    {
        [Key]
        public Guid UserId
        {
            get;
            set;
        }
        public string FullName
        {
            get;
            set;
        }
        public string Gender
        {
            get;
            set;
        }
        public DateTime BirthDate
        {
            get;
            set;
        }
        public string PhoneNumber
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Password
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
}
