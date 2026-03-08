using Accounts.Domain.Entities;

namespace Accounts.Application.Queries.Dto
{
    public record AccountLimitedDto(
        Guid AccountId,
        string Type,
        string Status,
        decimal Balance,
        string Currency,
        string NickName,
        DateTime OpenedAt)
    {
        public static AccountLimitedDto From(Account acc)
        {
            return new(
                acc.Id,
                acc.Type.ToString(),
                acc.Status.ToString(),
                acc.Balance.Amount,
                acc.Balance.Currency.ToString(),
                acc.NickName,
                acc.OpenedAt);
        }
    }
}
