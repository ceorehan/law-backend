using ZALaw.Api.Models;

namespace ZALaw.Api.DTOs;

public record DocumentSummary(
    Guid Id,
    string FileName,
    long SizeBytes,
    DocumentStatus Status,
    string? ReviewedBy,
    string? Comment,
    DateTime UploadedAt
);

public record ReviewDocumentRequest(DocumentStatus Status, string? Comment);
