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
}
