namespace Accounts.Domain.Entities
{
    public enum Currency
    {
        CAD,
        USD
    }

    public enum AccountType
    {
        Chequing,
        Savings
    }

    public enum AccountStatus
    {
        Active,
        Frozen,
        Closed
    }
}
