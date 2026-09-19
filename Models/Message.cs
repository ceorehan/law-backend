using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZALaw.Api.Models;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public TaxApplication? Application { get; set; }

    public Guid SenderId { get; set; }
    [ForeignKey(nameof(SenderId))]
    public User? Sender { get; set; }

    [Required, MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
