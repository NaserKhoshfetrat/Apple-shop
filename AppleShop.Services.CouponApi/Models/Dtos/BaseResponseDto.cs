namespace AppleShop.Services.CouponApi.Models.Dtos
{
    public class BaseResponseDto<T>
    {
        public bool IsSuccessful { get; set; } = true;
        public T? Result { get; set; }
        public IList<string> Error { get; set; } = new List<string>();
    }
}
