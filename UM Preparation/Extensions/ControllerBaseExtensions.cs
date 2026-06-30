using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UM_Preparation.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static int? GetCurrentUserId(this ControllerBase controller)
        {
            var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? controller.User.FindFirst("sub")?.Value
                ?? controller.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }

        public static string? GetCurrentUsername(this ControllerBase controller)
        {
            return controller.User.FindFirst("username")?.Value
                ?? controller.User.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static List<string> GetCurrentUserPermissions(this ControllerBase controller)
        {
            return [.. controller.User.FindAll("permissions").Select(c => c.Value)];
        }
    }
}