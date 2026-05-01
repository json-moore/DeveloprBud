using DeveloprBud.Data;
using DeveloprBud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DeveloprBud.Pages
{
    [Authorize]
    public class ArchiveModel : PageModel
    {
        // database connection
        private readonly AppDbContext _context;
        public ArchiveModel(AppDbContext context)
        {
            _context = context;
        }

        public List<TaskItem> CompletedTasks { get; set; }


        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ***** Retention policy - delete completed tasks that are older than 30 days to keep the database clean and performant *****
            var cutOffDate = DateTime.Now.AddDays(-30); // define when to delete a task

            var expiredTasks = _context.TaskItems
                .Where(t => t.UserId == userId &&
                            t.Status == "Completed" &&
                            t.CompletedDate < cutOffDate)
                .ToList(); // get tasks that are completed and past the cut off date

            if (expiredTasks.Any())
            {
                _context.TaskItems.RemoveRange(expiredTasks); // remove expired tasks from database and save changes
                _context.SaveChanges();
            }
            // ***** End of retention policy *****

            // expiredate = 

            // get completed tasks and order them by what was most recently completed
            CompletedTasks = _context.TaskItems
                .Where(t => t.UserId == userId &&
                            t.Status == "Completed")
                .OrderByDescending(t => t.CompletedDate)
                .ToList();
        }
    }
}