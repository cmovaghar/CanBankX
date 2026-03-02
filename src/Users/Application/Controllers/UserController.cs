using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commands;
using Users.Application.Requests;
using Users.Application.Results;
using Users.Domain.Exceptions;

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
        [ProducesResponseType(typeof(RegisterUserResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserRequest request,
            CancellationToken cancellationToken)
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
                request.PhoneNumber);

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "User registered successfully. UserId={UserId} Email={Email}",
                    result.UserId, result.Email);

                return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                return ValidationProblem(new ValidationProblemDetails(errors));
            }
            catch (UserDomainException ex) when (ex.Message.Contains("already exists"))
            {
                return Problem(
                    title: "Conflict",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}
