namespace Users.Domain.Exceptions
{
    public class UserDomainException : Exception
    {
        public UserDomainException(string message) : base(message) { }
        public UserDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
