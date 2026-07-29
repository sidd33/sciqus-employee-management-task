namespace EmployeeManagement.WebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.BUSINESS.Validations.Authorization;
using EmployeeManagement.DATA.DomainModels.TicketDATA;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IAuthorizationService _authorizationService;

    public TicketsController(ITicketService ticketService, IAuthorizationService authorizationService)
    {
        _ticketService = ticketService;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        if (currentUserRole == "Customer")
        {
            if (Guid.TryParse(currentUserId, out var customerId))
            {
                dto.CustomerId = customerId;
            }
        }
        else if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin" && currentUserId != dto.CustomerId.ToString())
        {
            return Forbid();
        }

        try
        {
            var ticket = await _ticketService.CreateTicketAsync(dto);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, ticket, new TicketOwnerOrAdminRequirement());
        if (!authResult.Succeeded) return Forbid();

        return Ok(ticket);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTickets([FromQuery] TicketQueryParameters query)
    {
        var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        if (currentUserRole == "Customer")
        {
            if (Guid.TryParse(currentUserId, out var customerId))
            {
                query.CustomerId = customerId;
            }
        }
        var tickets = await _ticketService.GetAllTicketsAsync(query);
        return Ok(tickets);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var existingTicket = await _ticketService.GetTicketByIdAsync(id);
        if (existingTicket == null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, existingTicket, new TicketOwnerOrAdminRequirement());
        if (!authResult.Succeeded) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        var isOwner = userId != null && existingTicket.CustomerId.ToString() == userId;
        var isAssignedEmployee = userId != null && existingTicket.AssignedEmployeeId?.ToString() == userId;

       
        if (isAssignedEmployee && !isAdmin && !isOwner)
        {
            if (!string.IsNullOrWhiteSpace(dto.Title) || 
                !string.IsNullOrWhiteSpace(dto.Description) || 
                dto.DepartmentId.HasValue || 
                dto.Status != TicketStatus.Completed || dto.Status != TicketStatus.InProgress)
            {
                return BadRequest(new { message = "Assigned employees are only permitted to update the ticket status to 'Completed'." });
            }
        }

        var updatedTicket = await _ticketService.UpdateTicketAsync(id, dto);
        return Ok(updatedTicket);
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AssignTicket(Guid id, [FromBody] AssignTicketDto dto)
    {
        try
        {
            var assignedTicket = await _ticketService.AssignTicketAsync(id, dto);
            if (assignedTicket == null) return NotFound();
            return Ok(assignedTicket);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var success = await _ticketService.DeleteTicketAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
