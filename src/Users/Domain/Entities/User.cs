using Shared;
using Users.Domain.Events;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class User : Entity, IAggregateRoot
    {
        public Email Email { get; private set; } = null!;
        public HashedPassword Password { get; private set; } = null!;
        public FullName FullName { get; private set; } = null!;
        public PhoneNumber PhoneNumber { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Address Address { get; private set; } = null!;
        public EncryptedSin Sin { get; private set; } = null!;

        public string EmailVerificationCode { get; private set; } = null!;

        public UserStatus Status { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }
        public TwoFactorMethod TwoFactorMethod { get; private set; }

        public Guid? InteracDepositAccount { get; private set; } = null;

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected User() { }

        private User(
            Guid id,
            Email email,
            string emailVerificationCode,
            HashedPassword password,
            FullName fullName,
            DateOnly dateOfBirth,
            Address address,
            EncryptedSin sin,
            PhoneNumber phoneNumber)
        {
            Id = id;
            Email = email;
            EmailVerificationCode = emailVerificationCode;
            Password = password;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Address = address;
            Sin = sin;
            PhoneNumber = phoneNumber;
            Status = UserStatus.PendingEmailVerification;
            IsTwoFactorEnabled = false;
            TwoFactorMethod = TwoFactorMethod.None;
            CreatedAt = DateTime.UtcNow;
        }

        public static User Register(
            Email email,
            HashedPassword password,
            FullName fullName,
            DateOnly dateOfBirth,
            Address address,
            EncryptedSin sin,
            PhoneNumber phoneNumber)
        {
            string emailVerificationCode = Random.Shared.Next(0, 1_000_000).ToString("D6");
            var user = new User(Guid.NewGuid(), email, emailVerificationCode, password, fullName, dateOfBirth, address, sin, phoneNumber);
            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, email.Value, emailVerificationCode, fullName.FirstName, fullName.LastName, dateOfBirth, address));
            return user;
        }

        public void ConfirmEmail()
        {
            if (Status != UserStatus.PendingEmailVerification) throw new EmailAlreadyConfirmedException(Id);

            Status = UserStatus.PendingKyc;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new EmailVerifiedDomainEvent(Id, DateTime.UtcNow));
        }

        public void StartKyc()
        {
            if (Status != UserStatus.PendingKyc) throw new InvalidKycStateException(Id, Status, nameof(StartKyc));

            Status = UserStatus.KycInProgress;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new KycStartedDomainEvent(Id, DateTime.UtcNow));
        }

        public void ApproveKyc()
        {
            if (Status != UserStatus.KycInProgress) throw new InvalidKycStateException(Id, Status, nameof(ApproveKyc));

            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new KycApprovedDomainEvent(Id, DateTime.UtcNow));
        }

        public void RejectKyc(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new KycRejectionReasonRequiredException(Id);

            Status = UserStatus.KycRejected;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new KycRejectedDomainEvent(Id, DateTime.UtcNow, reason));
        }

        public void EnableTwoFactor(TwoFactorMethod method)
        {
            if (Status != UserStatus.Active) throw new UserNotActiveException(Id);

            if (method == TwoFactorMethod.None) throw new InvalidTwoFactorMethodException(Id);

            IsTwoFactorEnabled = true;
            TwoFactorMethod = method;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new TwoFactorEnabledDomainEvent(Id, DateTime.UtcNow, method));
        }

        public void DisableTwoFactor()
        {
            IsTwoFactorEnabled = false;
            TwoFactorMethod = TwoFactorMethod.None;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new TwoFactorDisabledDomainEvent(Id, DateTime.UtcNow));
        }

        public void SetInteracDepositAccount(Guid accountId)
        {
            InteracDepositAccount = accountId;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new InteracDepositAccountChanged(accountId));
        }
    }
}
