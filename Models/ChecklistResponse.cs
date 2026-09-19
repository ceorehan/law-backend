using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZALaw.Api.Models;

/// <summary>
/// Flexible key/value store for checklist answers so new checklist fields
/// can be added without schema migrations. SectionKey groups fields into
/// the steps shown in the frontend stepper (e.g. "personal", "employment").
/// </summary>
public class ChecklistResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public TaxApplication? Application { get; set; }

    [Required, MaxLength(60)]
    public string SectionKey { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FieldKey { get; set; } = string.Empty;

    public string? Value { get; set; }

    public bool SectionCompleted { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
