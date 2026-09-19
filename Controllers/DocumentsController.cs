using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB, matches frontend copy

    public DocumentsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    private static DocumentSummary ToSummary(DocumentFile d) => new(
        d.Id, d.FileName, d.SizeBytes, d.Status, d.ReviewedBy?.FullName, d.Comment, d.UploadedAt
    );

    private async Task<TaxApplication?> GetAuthorizedApplication(Guid applicationId)
    {
        var application = await _db.Applications.FindAsync(applicationId);
        if (application is null) return null;

        if (User.GetRole() == UserRole.Client.ToString() && application.ClientId != User.GetUserId())
            return null;

        return application;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentSummary>>> GetDocuments(Guid applicationId)
    {
        if (await GetAuthorizedApplication(applicationId) is null) return NotFound();

        var documents = await _db.Documents
            .Include(d => d.ReviewedBy)
            .Where(d => d.ApplicationId == applicationId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return Ok(documents.Select(ToSummary));
    }

    [HttpPost]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<DocumentSummary>> Upload(Guid applicationId, IFormFile file)
    {
        if (file.Length == 0) return BadRequest(new { message = "File is empty." });
        if (file.Length > MaxFileSizeBytes) return BadRequest(new { message = "File exceeds the 10 MB limit." });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only PDF, JPG and PNG files are allowed." });

        var application = await GetAuthorizedApplication(applicationId);
        if (application is null) return NotFound();

        var uploadsRoot = Path.Combine(_env.ContentRootPath, "App_Data", "uploads", applicationId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var document = new DocumentFile
        {
            ApplicationId = applicationId,
            FileName = file.FileName,
            StoragePath = fullPath,
            SizeBytes = file.Length,
            Status = DocumentStatus.Uploaded,
        };

        _db.Documents.Add(document);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDocuments), new { applicationId }, ToSummary(document));
    }

    [HttpPatch("{documentId:guid}/review")]
    [Authorize(Roles = "Consultant,Admin")]
    public async Task<IActionResult> Review(Guid applicationId, Guid documentId, ReviewDocumentRequest request)
    {
        var document = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.ApplicationId == applicationId);
        if (document is null) return NotFound();

        document.Status = request.Status;
        document.Comment = request.Comment;
        document.ReviewedById = User.GetUserId();
        document.ReviewedAt = DateTime.UtcNow;

        var application = await _db.Applications.FindAsync(applicationId);
        if (application is not null)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = application.ClientId,
                Type = request.Status == DocumentStatus.ReplacementRequired
                    ? NotificationType.CorrectionRequired
                    : NotificationType.DocumentReview,
                Text = request.Status == DocumentStatus.ReplacementRequired
                    ? $"Please re-upload {document.FileName}: {request.Comment}"
                    : $"{document.FileName} was {request.Status.ToString().ToLowerInvariant()}.",
                RelatedApplicationId = applicationId,
            });
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
