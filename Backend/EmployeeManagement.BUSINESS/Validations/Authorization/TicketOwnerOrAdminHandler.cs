namespace EmployeeManagement.BUSINESS.Validations.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;

public class TicketOwnerOrAdminHandler : AuthorizationHandler<TicketOwnerOrAdminRequirement, TicketResponseDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TicketOwnerOrAdminRequirement requirement,
        TicketResponseDto resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = context.User.IsInRole("Admin");

        if (isAdmin || (userId != null && resource.CustomerId.ToString() == userId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
