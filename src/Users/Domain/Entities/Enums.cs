namespace Users.Domain.Entities
{
    public enum UserStatus
    {
        PendingVerification,
        Active,
        Suspended,
        Closed
    }

    public enum KycStatus
    {
        NotStarted,
        InProgress,
        Approved,
        Rejected
    }

    public enum TwoFactorMethod
    {
        None,
        Totp,
        Sms
    }
}
