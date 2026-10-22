namespace AppleShop.Services.ShoppingCartApi
{
    public class ProjectConfigurations
    {
        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";

        public static readonly (string Admin, string Customer) Role =
            (
                Admin: RoleAdmin,
                Customer: RoleCustomer
            );

        public static (string ClientId, string ApiScope) ClientWebProject =
            (
                ClientId: "WebProject",
                ApiScope: "apple"
            );
    }
}
