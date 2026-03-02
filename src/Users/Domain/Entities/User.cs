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
        public PhoneNumber? PhoneNumber { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public Address Address { get; private set; } = null!;

        public UserStatus Status { get; private set; }
        public KycStatus KycStatus { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }
        public TwoFactorMethod TwoFactorMethod { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected User() { } // Nécessaire pour EF Core

        private User(
            Guid id,
            Email email,
            HashedPassword password,
            FullName fullName,
            DateOnly dateOfBirth,
            Address address,
            PhoneNumber? phoneNumber)
        {
            Id = id;
            Email = email;
            Password = password;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Address = address;
            PhoneNumber = phoneNumber;
            Status = UserStatus.PendingVerification;
            KycStatus = KycStatus.NotStarted;
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
            PhoneNumber? phoneNumber = null)
        {
            var user = new User(Guid.NewGuid(), email, password, fullName, dateOfBirth, address, phoneNumber);
            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, email.Value, fullName.FirstName, fullName.LastName, dateOfBirth, address));
            return user;
        }

        public void ApproveKyc()
        {
            if (KycStatus == KycStatus.Approved) throw new UserDomainException("KYC is already approved.");

            KycStatus = KycStatus.Approved;
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new KycApprovedDomainEvent(Id));
        }

        public void RejectKyc(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new UserDomainException("A rejection reason is required.");

            KycStatus = KycStatus.Rejected;
            Status = UserStatus.Suspended;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new KycRejectedDomainEvent(Id, reason));
        }

        public void EnableTwoFactor(TwoFactorMethod method)
        {
            if (Status != UserStatus.Active) throw new UserDomainException("Two-factor authentication can only be enabled on active accounts.");

            if (method == TwoFactorMethod.None) throw new UserDomainException("A valid two-factor method must be specified.");

            if (method == TwoFactorMethod.Sms && PhoneNumber is null) throw new UserDomainException("A verified phone number is required for SMS-based 2FA.");

            IsTwoFactorEnabled = true;
            TwoFactorMethod = method;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new TwoFactorEnabledDomainEvent(Id, method));
        }

        public void DisableTwoFactor()
        {
            IsTwoFactorEnabled = false;
            TwoFactorMethod = TwoFactorMethod.None;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
