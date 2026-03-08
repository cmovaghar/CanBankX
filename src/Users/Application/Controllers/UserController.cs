using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Users.Application.Commands.LoginUser;
using Users.Application.Commands.RegisterUser;
using Users.Application.Commands.SetupInteracDeposit;

namespace Users.Application.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UsersController(IMediator mediator, ILogger<UsersController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<UsersController> _logger = logger;

        [HttpPost("register")]
        [PublicEndpoint]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Street,
                request.City,
                request.Province,
                request.PostalCode,
                request.Sin,
                request.PhoneNumber);

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User registered successfully. UserId={UserId} Email={Email}", result.UserId, result.Email);

            return CreatedAtAction(nameof(Register), new { result.Token });
        }

        [HttpPost("login")]
        [PublicEndpoint]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User logged-in successfully. UserId={UserId} Email={Email}", result.UserId, result.Email);
            return Ok(new { token = result.Token });
        }

        [HttpGet("setup-interac")]
        [LoginEndpoint(JwtPurposes.Login)]
        public async Task<IActionResult> GetInterac([FromBody] SetupInteracDepositRequest request, CancellationToken cancellationToken)
        {
            Guid userId = User.GetUserId();
            var command = new SetupInteracDepositCommand(userId, request.AccountId);
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User {UserId} set interac deposit account {AccountId}.", userId, result.AccountId);
            return Ok(result.AccountId);
        }


        [HttpPost("setup-interac")]
        [LoginEndpoint(JwtPurposes.Login)]
        public async Task<IActionResult> SetupInterac([FromBody] SetupInteracDepositRequest request, CancellationToken cancellationToken)
        {
            Guid userId = User.GetUserId();
            var command = new SetupInteracDepositCommand(userId, request.AccountId);
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User {UserId} set interac deposit account {AccountId}.", userId, result.AccountId);
            return Ok(result.AccountId);
        }
    }
}
