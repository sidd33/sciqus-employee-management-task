using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Security.Claims;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.WebAPI.Authorization
{
	public class SameUserOrAdminHandler
		: AuthorizationHandler<SameUserOrAdminRequirement, Guid>
	{
		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			SameUserOrAdminRequirement requirement,
			Guid targetUserId)
		{
			var role = context.User.FindFirstValue(ClaimTypes.Role);

			var currentUserId =
				context.User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Admin can access everything
			if (role == EmployeeRole.Admin.ToString())
			{
				context.Succeed(requirement);
				return Task.CompletedTask;
			}

			// Employee can access only their own record
			if (Guid.TryParse(currentUserId, out var id) &&
				id == targetUserId)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
