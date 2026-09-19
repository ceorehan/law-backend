using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZALaw.Api.Models;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public User? Client { get; set; }

    public Guid? ConsultantId { get; set; }
    [ForeignKey(nameof(ConsultantId))]
    public User? Consultant { get; set; }

    public Guid? ApplicationId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public DateTime ScheduledAt { get; set; }

    public AppointmentType Type { get; set; } = AppointmentType.VideoCall;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
