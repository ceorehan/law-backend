namespace ZALaw.Api.DTOs;

public record ChecklistFieldDto(string SectionKey, string FieldKey, string? Value);

public record SaveChecklistSectionRequest(
    string SectionKey,
    Dictionary<string, string?> Fields,
    bool SectionCompleted
);

public record ChecklistStateDto(
    string SectionKey,
    bool Completed,
    Dictionary<string, string?> Fields
);
