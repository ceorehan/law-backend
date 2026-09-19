using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/checklist")]
[Authorize]
public class ChecklistController : ControllerBase
{
    private readonly AppDbContext _db;

    public ChecklistController(AppDbContext db)
    {
        _db = db;
    }

    private async Task<TaxApplication?> GetAuthorizedApplication(Guid applicationId)
    {
        var application = await _db.Applications.FindAsync(applicationId);
        if (application is null) return null;

        if (User.GetRole() == UserRole.Client.ToString() && application.ClientId != User.GetUserId())
            return null;

        return application;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChecklistStateDto>>> GetChecklist(Guid applicationId)
    {
        var application = await GetAuthorizedApplication(applicationId);
        if (application is null) return NotFound();

        var responses = await _db.ChecklistResponses
            .Where(c => c.ApplicationId == applicationId)
            .ToListAsync();

        var sections = responses
            .GroupBy(r => r.SectionKey)
            .Select(g => new ChecklistStateDto(
                g.Key,
                g.All(r => r.SectionCompleted),
                g.ToDictionary(r => r.FieldKey, r => r.Value)
            ));

        return Ok(sections);
    }

    [HttpPut("{sectionKey}")]
    public async Task<IActionResult> SaveSection(Guid applicationId, string sectionKey, SaveChecklistSectionRequest request)
    {
        var application = await GetAuthorizedApplication(applicationId);
        if (application is null) return NotFound();

        var existing = await _db.ChecklistResponses
            .Where(c => c.ApplicationId == applicationId && c.SectionKey == sectionKey)
            .ToListAsync();

        foreach (var (fieldKey, value) in request.Fields)
        {
            var record = existing.FirstOrDefault(c => c.FieldKey == fieldKey);
            if (record is null)
            {
                _db.ChecklistResponses.Add(new ChecklistResponse
                {
                    ApplicationId = applicationId,
                    SectionKey = sectionKey,
                    FieldKey = fieldKey,
                    Value = value,
                    SectionCompleted = request.SectionCompleted,
                    UpdatedAt = DateTime.UtcNow,
                });
            }
            else
            {
                record.Value = value;
                record.SectionCompleted = request.SectionCompleted;
                record.UpdatedAt = DateTime.UtcNow;
            }
        }

        application.LastUpdated = DateTime.UtcNow;

        var totalSections = 12; // matches frontend checklistSteps.length
        var completedSections = await _db.ChecklistResponses
            .Where(c => c.ApplicationId == applicationId)
            .Select(c => c.SectionKey)
            .Distinct()
            .CountAsync();
        application.Progress = Math.Min(100, (int)Math.Round(completedSections * 100.0 / totalSections));

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
