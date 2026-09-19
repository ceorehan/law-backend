namespace ZALaw.Api.DTOs;

public record SendMessageRequest(string Body);

public record MessageSummary(Guid Id, string SenderName, bool IsFromClient, string Body, DateTime SentAt);
