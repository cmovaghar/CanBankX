namespace Transactions.Domain.Entities
{
    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum Currency
    {
        CAD,
        USD
    }

    public enum TransactionDestinationType
    {
        Account,
        Interac
    }
}
