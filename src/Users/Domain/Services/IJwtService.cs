namespace Users.Domain.Services
{
    public interface IJwtService
    {
        string GenerateEmailValidationToken(Guid userId, string email);
        string GenerateLoginToken(Guid userId, string email, bool login);
    }
}
