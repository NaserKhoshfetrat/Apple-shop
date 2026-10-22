namespace AppleShop.Web
{
    public class ProjectConfigurations
    {
        public static readonly (string Admin, string Customer) Role =
          (
              Admin: "Admin",
              Customer: "Customer"
          );

        public static (string ClientId, string ApiScope, string Secret) ClientWebProject =
           (
               ClientId: "WebProject",
               ApiScope: "apple",
               Secret: "secret"
           );
    }
}