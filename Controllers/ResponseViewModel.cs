using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryAPI.Controllers
{
    internal class ResponseViewModel : ModelStateDictionary
    {
        public object Status { get; set; }
        public object Message { get; set; }
    }
}