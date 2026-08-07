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

    public Role? Role { get; set; } = null;

    public ICollection<RefreshToken> RefreshTokens
    {
        get;
        set;
    } = new List<RefreshToken>();
    public string? PasswordResetToken
    {
        get;
        set;
    }

    public DateTime? PasswordResetTokenExpiry
    {
        get;
        set;
    }
    public string? EmailVerificationToken
    {
        get;
        set;
    }
    public int FailedLoginAttempts
{
    get;
    set;
}

public DateTime? LockoutEndTime
{
    get;
    set;
}
}