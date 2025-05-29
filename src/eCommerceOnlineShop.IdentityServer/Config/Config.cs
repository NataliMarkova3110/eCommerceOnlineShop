using Duende.IdentityServer.Models;

namespace eCommerceOnlineShop.IdentityServer.Config
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "User role(s)", ["role"])
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("catalog", "catalog API"),
                new ApiScope("cart", "cart API"),
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                new ApiResource("catalog", "catalog API"),
                new ApiResource("cart", "cart API")
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client
                {
                    ClientId = "ecommerceclient",
                    ClientName = "eCommerce Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "openid", "profile", "roles", "catalog", "cart" },
                    AccessTokenLifetime = 3600,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 1296000,
                    AllowOfflineAccess = true
                }
            ];
    }
}