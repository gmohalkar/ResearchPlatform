using MediatR;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string RoleName
) : IRequest<Guid>;