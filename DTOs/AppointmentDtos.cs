using ZALaw.Api.Models;

namespace ZALaw.Api.DTOs;

public record CreateAppointmentRequest(string Title, DateTime ScheduledAt, AppointmentType Type, Guid? ApplicationId);

public record AppointmentSummary(Guid Id, string Title, DateTime ScheduledAt, AppointmentType Type, AppointmentStatus Status, string? ConsultantName);
