using Users.Domain.Entities;

namespace Users.Domain.Exceptions
{
    public class EmailAlreadyConfirmedException(Guid userId) : Exception($"User {userId} email has already been confirmed.");
    public class InvalidKycStateException(Guid userId, UserStatus currentStatus, string operation) : Exception($"Cannot perform '{operation}' on user {userId} with status {currentStatus}.");
    public class UserNotActiveException(Guid userId) : Exception($"User {userId} must be active to perform this operation.");
    public class InvalidTwoFactorMethodException(Guid userId) : Exception($"A valid two-factor method must be specified for user {userId}.");
    public class KycRejectionReasonRequiredException(Guid userId) : Exception($"A rejection reason is required when rejecting KYC for user {userId}.");
    public class UserNotFoundException(Guid userId) : Exception($"User {userId} was not found.");
    public class UserDoesNotOwnAccountException(Guid userId, Guid accountId) : Exception($"User {userId} does not own account {accountId}.");

    public class UserEmailAlreadyInUseException(string email) : Exception($"The email {email} is already in use.");
    public class UserFailedLoginException() : Exception("Invalid email or password.");

    // 400, can expose to user
    public class DomainValidationException(string field, string message) : Exception(message)
    {
        public string Field { get; } = field;
    }

    // 500, do not expose to user
    public class ValueEmptyException(string field, string message) : Exception(message)
    {
        public string Field { get; } = field;
    }
}
