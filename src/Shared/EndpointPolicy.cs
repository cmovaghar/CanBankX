namespace Shared
{
    public static class JwtPurposes
    {
        public const string VerifyEmail = "verify-email";
        public const string Login = "login";
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PublicEndpointAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LoginEndpointAttribute(params string[] allowedPurposes) : Attribute 
    {
        public string[] AllowedPurposes { get; } = allowedPurposes;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalEndpointAttribute : Attribute { }
}
