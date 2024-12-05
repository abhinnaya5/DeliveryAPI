namespace DeliveryAPI.ViewModel
{
    public static class ResponseCode
    {
        public static readonly int SuccessCode = 200;
        public static readonly string SuccessMessage = "Success";

        public static readonly int BadRequestErrorCode = 400;
        public static readonly string BadRequestErrorMessage = "Bad request";

        public static readonly int InternalErrorCode = 500;
        public static readonly string InternalErrorMessage = "Internal error";
    }
    public class BaseResponse
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public BaseResponse(int status, string message)
        {
            Status = status;
            Message = message;
        }
    }
    public class PaginationViewModel
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; }
    }
}
