using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ApplicationsController(AppDbContext db)
    {
        _db = db;
    }

    private static ApplicationSummary ToSummary(TaxApplication a) => new(
        a.Id, a.ApplicationNumber, a.Client?.FullName ?? string.Empty, a.Client?.Cnic,
        a.TaxYear, a.Service, a.Status, a.Progress, a.AssignedConsultant?.FullName,
        a.SubmittedDate, a.LastUpdated
    );

    /// <summary>Client: my applications. Consultant/Admin: all applications.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationSummary>>> GetApplications()
    {
        var role = User.GetRole();
        var userId = User.GetUserId();

        var query = _db.Applications.Include(a => a.Client).Include(a => a.AssignedConsultant).AsQueryable();

        if (role == UserRole.Client.ToString())
            query = query.Where(a => a.ClientId == userId);

        var applications = await query.OrderByDescending(a => a.LastUpdated).ToListAsync();
        return Ok(applications.Select(ToSummary));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApplicationSummary>> GetApplication(Guid id)
    {
        var application = await _db.Applications
            .Include(a => a.Client).Include(a => a.AssignedConsultant)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null) return NotFound();

        if (User.GetRole() == UserRole.Client.ToString() && application.ClientId != User.GetUserId())
            return Forbid();

        return Ok(ToSummary(application));
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationSummary>> CreateApplication(CreateApplicationRequest request)
    {
        var userId = User.GetUserId();
        var sequence = await _db.Applications.CountAsync(a => a.TaxYear == request.TaxYear) + 1;

        var application = new TaxApplication
        {
            ClientId = userId,
            Service = request.Service,
            TaxYear = request.TaxYear,
            ApplicationNumber = ApplicationNumberGenerator.Generate(request.TaxYear, sequence),
            Status = ApplicationStatus.New,
            Progress = 0,
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();

        await _db.Entry(application).Reference(a => a.Client).LoadAsync();
        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, ToSummary(application));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Consultant,Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateApplicationStatusRequest request)
    {
        var application = await _db.Applications.FindAsync(id);
        if (application is null) return NotFound();

        application.Status = request.Status;
        application.LastUpdated = DateTime.UtcNow;
        if (request.Status == ApplicationStatus.Submitted)
            application.SubmittedDate ??= DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _db.Notifications.Add(new Notification
        {
            UserId = application.ClientId,
            Type = NotificationType.StatusChange,
            Text = $"Your application {application.ApplicationNumber} status changed to {request.Status}.",
            RelatedApplicationId = application.Id,
        });
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "Consultant,Admin")]
    public async Task<IActionResult> AssignConsultant(Guid id, AssignConsultantRequest request)
    {
        var application = await _db.Applications.FindAsync(id);
        if (application is null) return NotFound();

        var consultant = await _db.Users.FindAsync(request.ConsultantId);
        if (consultant is null || consultant.Role is not (UserRole.Consultant or UserRole.Admin))
            return BadRequest(new { message = "Selected user is not a consultant." });

        application.AssignedConsultantId = request.ConsultantId;
        application.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
