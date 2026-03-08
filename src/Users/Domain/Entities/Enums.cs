namespace Users.Domain.Entities
{
    public enum UserStatus
    {
        PendingEmailVerification,
        PendingKyc,
        KycInProgress,
        KycRejected,
        Active,
        Suspended,
        Closed
    }

    public enum TwoFactorMethod
    {
        None,
        Totp,
        Sms
    }
}
