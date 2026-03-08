using Accounts.Application.Commands.OpenAccount;
using Accounts.Application.Commands.TransferFunds;
using Accounts.Application.Queries.GetUserAccounts;
using Accounts.Application.Queries.VerifyOwnership;
using Accounts.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Accounts.Application.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Produces("application/json")]
    public class AccountController(IMediator mediator, ILogger<AccountController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<AccountController> _logger = logger;

        [HttpGet]
        [LoginEndpoint(JwtPurposes.Login)]
        public async Task<IActionResult> GetAllAccounts(CancellationToken cancellationToken)
        {
            Guid userId = User.GetUserId();

            var command = new GetUserAccountsQuery(userId);

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Retrieved information for [{Amount}] open accounts for user [{UserId}].", result.Accounts.Count, result.UserId);

            return Ok(new { result.Accounts });
        }

        [HttpPost]
        [LoginEndpoint(JwtPurposes.Login)]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccountRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            var command = new OpenAccountCommand(userId, request.Type, Domain.Entities.Currency.CAD);

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Account {AccountId} opened successfully by User {UserId}.", result.AccountId, result.UserId);

            return CreatedAtAction(nameof(OpenAccount), new { result.AccountId });
        }

        [HttpPost("transfer")]
        [InternalEndpoint]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequest request, CancellationToken cancellationToken)
        {
            Money money = new(request.Amount, request.Currency);
            var command = new TransferFundsCommand(request.From, request.To, money);

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Transfer successful. [{money}] transfered from account [{from}] to account [{to}].", result.Amount.ToString(), result.From, result.To);

            return Ok();
        }

        [HttpPost("verify-ownership")]
        [InternalEndpoint]
        public async Task<IActionResult> VerifyOwnership([FromBody] VerifyOwnershipRequest request, CancellationToken cancellationToken)
        {
            var query = new VerifyOwnershipQuery(request.UserId, request.AccountId);

            var result = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Ownership of account verified. {UserId} owns {AccountId} : {Result}", query.UserId.ToString(), query.AccountId, result.IsOwner);

            return Ok(result.IsOwner);
        }
    }
}
