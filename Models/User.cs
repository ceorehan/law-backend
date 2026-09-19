using System.ComponentModel.DataAnnotations;

namespace ZALaw.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Cnic { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public UserRole Role { get; set; } = UserRole.Client;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<TaxApplication> ApplicationsAsClient { get; set; } = new List<TaxApplication>();
    public ICollection<TaxApplication> ApplicationsAsConsultant { get; set; } = new List<TaxApplication>();
}
