using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;

    public MessagesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageSummary>>> GetMessages(Guid applicationId)
    {
        var messages = await _db.Messages
            .Include(m => m.Sender)
            .Where(m => m.ApplicationId == applicationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages.Select(m => new MessageSummary(
            m.Id, m.Sender?.FullName ?? string.Empty, m.Sender?.Role == UserRole.Client, m.Body, m.SentAt
        )));
    }

    [HttpPost]
    public async Task<ActionResult<MessageSummary>> SendMessage(Guid applicationId, SendMessageRequest request)
    {
        var application = await _db.Applications.FindAsync(applicationId);
        if (application is null) return NotFound();

        var senderId = User.GetUserId();
        var sender = await _db.Users.FindAsync(senderId);

        var message = new Message
        {
            ApplicationId = applicationId,
            SenderId = senderId,
            Body = request.Body,
        };
        _db.Messages.Add(message);

        var recipientId = sender?.Role == UserRole.Client
            ? application.AssignedConsultantId
            : application.ClientId;

        if (recipientId.HasValue)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = recipientId.Value,
                Type = NotificationType.Message,
                Text = $"New message on {application.ApplicationNumber}.",
                RelatedApplicationId = applicationId,
            });
        }

        await _db.SaveChangesAsync();

        return Ok(new MessageSummary(message.Id, sender?.FullName ?? string.Empty, sender?.Role == UserRole.Client, message.Body, message.SentAt));
    }
}
