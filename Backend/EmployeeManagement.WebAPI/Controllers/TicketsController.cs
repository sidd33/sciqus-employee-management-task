namespace EmployeeManagement.WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        var ticket = await _ticketService.CreateTicketAsync(dto);
        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var updatedTicket = await _ticketService.UpdateTicketAsync(id, dto);
        if (updatedTicket == null) return NotFound();
        return Ok(updatedTicket);
    }
}
