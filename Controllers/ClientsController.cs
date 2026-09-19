using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.Models;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize(Roles = "Consultant,Admin")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _db.Users
            .Where(u => u.Role == UserRole.Client)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Cnic,
                u.Email,
                u.CreatedAt,
                ApplicationCount = u.ApplicationsAsClient.Count,
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(clients);
    }
}
