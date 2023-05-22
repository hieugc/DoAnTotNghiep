namespace DoAnTotNghiep.Enum
{
    public class Scheme
    {
        public static string AuthenticationCookie() => "SecurityCookieScheme";
        public static string AuthenticationJWT() => "SecurityJWTScheme";
        public static string Authentication() => "SecurityScheme";
    }

    public class ConfigurationJWT
    {
        public static string JwtBearerIssuer() => "JwtBearerIssuer";
        public static string JwtBearerAudience() => "JwtBearerAudience";
        public static string JwtBearerIssuerSigningKey() => "JwtBearerIssuerSigningKey";
    }

    public class ConfigurationEmail
    {
        public static string Email() => "ModuleEmail";
        public static string Password() => "ModulePassword";
    }

    public class SystemKey
    {
        public static string Base64() => "SystemBase64";
        public static string WebsiteKey() => "WebsiteKey";
    }

    public static class ZaloPay
    {
        public const string ZaloId = "ZaloId";
        public const string ZaloKey1 = "ZaloKey1";
        public const string ZaloKey2 = "ZaloKey2";
    }

    public static class BingMaps
    {
        public const string Key = "BingMapsKey";
    }
}
