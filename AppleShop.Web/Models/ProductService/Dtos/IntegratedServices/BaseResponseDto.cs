namespace AppleShop.Web.Models.ProductService.Dtos.IntegratedServices
{
    public class BaseResponseDto<T>
    {
        public bool IsSuccessful { get; set; } = true;
        public T? Result { get; set; }
        public IList<string> Error { get; set; } = new List<string>();
    }
}
