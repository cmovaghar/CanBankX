using MediatR;
using Transactions.Domain.Entities;
using Transactions.Domain.Events;

namespace Transactions.Application.EventHandlers
{
    public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedDomainEvent>
    {
        public Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
        {

            Guid destination = notification.Transaction.DestinationType switch
            {
                TransactionDestinationType.Account => Guid.Parse(notification.Transaction.Destination),

                TransactionDestinationType.Interac => ,

                _ => throw new InvalidOperationException($"Unknown destination type: {notification.Transaction.DestinationType}")
            };

            return Task.CompletedTask;
        }
    }
}
