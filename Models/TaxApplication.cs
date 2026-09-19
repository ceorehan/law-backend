using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZALaw.Api.Models;

public class TaxApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(30)]
    public string ApplicationNumber { get; set; } = string.Empty;

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public User? Client { get; set; }

    public Guid? AssignedConsultantId { get; set; }
    [ForeignKey(nameof(AssignedConsultantId))]
    public User? AssignedConsultant { get; set; }

    [Required, MaxLength(120)]
    public string Service { get; set; } = string.Empty;

    public int TaxYear { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;

    [Range(0, 100)]
    public int Progress { get; set; } = 0;

    public DateTime? SubmittedDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public ICollection<ChecklistResponse> ChecklistResponses { get; set; } = new List<ChecklistResponse>();
    public ICollection<DocumentFile> Documents { get; set; } = new List<DocumentFile>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
