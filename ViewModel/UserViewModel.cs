using DeliveryAPI.ViewModel;

namespace DeliveryAPI.Models.Domain
{
    public class AddUserViewModel
    {
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
    public class EditUserViewModel
    {
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
        public string Address
        {
            get;
            set;
        }
    }

    public class LoginViewModel
    {
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
    }

    public class TokenResponse
    {
        public string Token
        {
            get;
            set;
        }
    }
}
