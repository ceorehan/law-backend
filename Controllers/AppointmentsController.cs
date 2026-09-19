using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AppointmentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentSummary>>> GetAppointments()
    {
        var role = User.GetRole();
        var userId = User.GetUserId();

        var query = _db.Appointments.Include(a => a.Consultant).AsQueryable();
        query = role == UserRole.Client.ToString()
            ? query.Where(a => a.ClientId == userId)
            : query.Where(a => a.ConsultantId == userId || a.ConsultantId == null);

        var appointments = await query.OrderBy(a => a.ScheduledAt).ToListAsync();

        return Ok(appointments.Select(a => new AppointmentSummary(
            a.Id, a.Title, a.ScheduledAt, a.Type, a.Status, a.Consultant?.FullName
        )));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentSummary>> Create(CreateAppointmentRequest request)
    {
        var appointment = new Appointment
        {
            ClientId = User.GetUserId(),
            Title = request.Title,
            ScheduledAt = request.ScheduledAt,
            Type = request.Type,
            ApplicationId = request.ApplicationId,
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return Ok(new AppointmentSummary(appointment.Id, appointment.Title, appointment.ScheduledAt, appointment.Type, appointment.Status, null));
    }
}
