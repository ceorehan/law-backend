using ZALaw.Api.Models;

namespace ZALaw.Api.DTOs;

public record ApplicationSummary(
    Guid Id,
    string ApplicationNumber,
    string ClientName,
    string? Cnic,
    int TaxYear,
    string Service,
    ApplicationStatus Status,
    int Progress,
    string? AssignedConsultant,
    DateTime? SubmittedDate,
    DateTime LastUpdated
);

public record CreateApplicationRequest(string Service, int TaxYear);

public record UpdateApplicationStatusRequest(ApplicationStatus Status, string? Note);

public record AssignConsultantRequest(Guid ConsultantId);
