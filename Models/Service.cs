using System.ComponentModel.DataAnnotations;

namespace ZALaw.Api.Models;

public class Service
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(80)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string ShortDescription { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Icon { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
}
