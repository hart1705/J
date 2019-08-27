using Microsoft.AspNetCore.Identity;
using PORTAL.DAL.EF;
using PORTAL.DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PORTAL.WEB.Helpers
{
    public static class Common
    {
        public static async Task<ApplicationUser> GetCurrentUser(UserManager<ApplicationUser> userManager, ClaimsPrincipal userClaims)
        {
            var currentUser = await userManager.GetUserAsync(userClaims);
            return currentUser;
        }

        public static async Task<List<string>> GetUserRoles(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            List<string> userRoles = (List<string>)await userManager.GetRolesAsync(user);
            return userRoles;
        }

        public static ApplicationUser ResolveUser(ApplicationDbContext context, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = context.ApplicationUser.Find(userId);
                if (user != null)
                {
                    return user;
                }
            }
            return null;
        }

        public static string TimeAgo(DateTime? dt)
        {
            DateTime date = DateTime.Now;
            if (dt == null)
            {
                return string.Empty;
            }
            else
            {
                date = dt.Value;
            }
            TimeSpan span = DateTime.Now - date;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} {1} ago",
                years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("about {0} {1} ago",
                months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return String.Format("about {0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("about {0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return String.Format("about {0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "Just now";
            return string.Empty;
        }

        public static string FileSize(int? fileSize)
        {
            if (fileSize.HasValue)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                int order = 0;
                while (fileSize >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    fileSize = fileSize / 1024;
                }

                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                // show a single decimal place, and no space.
                return String.Format("{0:0.##} {1}", fileSize, sizes[order]);
            }
            return string.Empty;
        }
    }
}
