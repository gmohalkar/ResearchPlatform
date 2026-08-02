namespace IdentityService.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public bool IsLocked { get; set; }

    public DateTime LastLoginDate { get; set; }

    public Guid RoleId { get; set; }

    public Role Role { get; set; } 

    public ICollection<RefreshToken> RefreshTokens
{
get;
set;
} = new List<RefreshToken>();
}