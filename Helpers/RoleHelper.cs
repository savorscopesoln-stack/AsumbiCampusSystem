using Microsoft.AspNetCore.Http;
using System.Linq;

namespace AsumbiCampusSystem.Helpers
{
    public static class RoleHelper
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Session.GetString("Role"));
        }

        public static string GetRole(HttpContext context)
        {
            return context.Session.GetString("Role") ?? "";
        }

        public static bool HasAnyRole(HttpContext context, params string[] roles)
        {
            var currentRole = GetRole(context);
            return roles.Contains(currentRole);
        }
    }
}