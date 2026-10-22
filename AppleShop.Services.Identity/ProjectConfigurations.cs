using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace AppleShop.Services.Identity
{
    public class ProjectConfigurations
    {
        public static readonly (string Admin, string Customer) Role =
          (
              Admin: "Admin",
              Customer: "Customer"
          );

        //todo replace "secret" with another secure key
        private static readonly (string ClientId, IReadOnlyList<string> ApiScopes, string Secret) ClientGeneral =
            (
                ClientId: "client",
                ApiScopes: new[] { "read", "write", "profile" },
                Secret: "secret"
            );
        private static (string ClientId, string ApiScope, string Secret) ClientWebProject = 
            (
                ClientId: "WebProject",
                ApiScope: "apple",
                Secret: "secret"
            );

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> {
                new ApiScope(name: ClientWebProject.ApiScope,   displayName: "apple Server"), //give user full access to all operations
                //new ApiScope(name: "read",   displayName: "Read your data."),
                //new ApiScope(name: "write",  displayName: "Write your data."),
                //new ApiScope(name: "delete", displayName: "Delete your data.")
            };

        public static IEnumerable<Client> GetClients(string identityServerUrl)
        {
            var list = new List<Client>
            {
                new Client
                {
                    ClientId= ClientGeneral.ClientId,
                    ClientSecrets= { new Secret(ClientGeneral.Secret.Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes=ClientGeneral.ApiScopes.ToArray(),
                },
                new Client
                {
                    ClientId= ClientWebProject.ClientId,
                    ClientSecrets= { new Secret(ClientWebProject.Secret.Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris={ $"{identityServerUrl}/signin-oidc" },
                    PostLogoutRedirectUris={$"{identityServerUrl}/signout-callback-oidc" },
                    AllowedScopes=new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        ClientWebProject.ApiScope
                    }
                },
            };

            return list;
        }
    }
}
