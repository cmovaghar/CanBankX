using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Transactions.Application.Commands;
using Transactions.Domain.ValueObjects;

namespace Transactions.Application.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionController(IMediator mediator, ILogger<TransactionController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<TransactionController> _logger = logger;


        [HttpPost]
        [LoginEndpoint(JwtPurposes.Login)]
        public async Task<IActionResult> InitiateTransaction([FromBody] InitiateTransactionRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var money = new Money(request.Amount, request.Currency);

            var command = new InitiateTransactionCommand(
                request.IdempotencyKey,
                userId, 
                request.FromAccount,
                request.DestinationType,
                request.Destination,
                money);

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Transaction {TransactionId} initiated by {UserId}.", result.Transaction.Id, result.Transaction.FromUser);

            return CreatedAtAction(nameof(InitiateTransaction), new { result.Transaction.Id });
        }
    }
}
