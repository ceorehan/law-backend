using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZALaw.Api.Models;

public class DocumentFile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public TaxApplication? Application { get; set; }

    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;

    public Guid? ReviewedById { get; set; }
    [ForeignKey(nameof(ReviewedById))]
    public User? ReviewedBy { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}
