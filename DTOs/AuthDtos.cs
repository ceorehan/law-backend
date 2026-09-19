using System.ComponentModel.DataAnnotations;

namespace ZALaw.Api.DTOs;

public record RegisterRequest(
    [Required, MaxLength(150)] string FullName,
    [Required, MaxLength(20)] string Cnic,
    [Required, MaxLength(20)] string Phone,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(string Token, UserSummary User);

public record UserSummary(Guid Id, string FullName, string Email, string Role);
