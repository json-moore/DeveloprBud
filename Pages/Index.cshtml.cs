using DeveloprBud.Data;
using DeveloprBud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeveloprBud.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        // database connection
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalTasksOpen { get; set; } // total tasks open - displayed in dashboard
        public int TotalExistingSnippets { get; set; } // total code snippets - displayed in dashboard
        public TaskItem? LongestTaskOpen { get; set; } // longest task open - displayed in dashboard
        public int? LongestTaskDayCount { get; set; } // longest task open day count - displayed in dashboard
        public int TasksCompletedToday { get; set; } // tasks completed today - displayed in dashboard
        public int TasksCompletedThisWeek { get; set; } // tasks completed this week - displayed in dashboard
        public int TasksCompletedThisMonth { get; set; } // tasks completed this month - displayed in dashboard
        public CodeSnippet? LastSnippetSaved { get; set; } // last snippet saved - displayed in dashboard



        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TotalTasksOpen = await _context.TaskItems
                .CountAsync(t => t.UserId == userId && t.CompletedDate == null);
            TotalExistingSnippets = await _context.CodeSnippets
                .CountAsync(s => s.UserId == userId);
            TasksCompletedToday = await _context.TaskItems
                .CountAsync(t => t.UserId == userId &&
                                 t.CompletedDate != null &&
                                 t.CompletedDate >= DateTime.Today);
            TasksCompletedThisWeek = await _context.TaskItems
                .CountAsync(t => t.UserId == userId &&
                                 t.CompletedDate != null &&
                                 t.CompletedDate >= DateTime.Today.AddDays(-7));
            TasksCompletedThisMonth = await _context.TaskItems
                .CountAsync(t => t.UserId == userId &&
                                 t.CompletedDate != null &&
                                 t.CompletedDate >= DateTime.Today.AddMonths(-1));

            // get the oldest task that is still open
            LongestTaskOpen = await _context.TaskItems
                .Where(t => t.UserId == userId && t.CompletedDate == null)
                .OrderBy(t => t.CreatedDate)
                .FirstOrDefaultAsync(); // returns only one task

            // calculate how many days the longest open task has been open
            if (LongestTaskOpen != null)
            {
                // real time date <minus> create date time length
                LongestTaskDayCount = (DateTime.Now - LongestTaskOpen.CreatedDate).Days;
            }

            // get the most recently saved code snippet
            LastSnippetSaved = await _context.CodeSnippets
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedDate)
                .FirstOrDefaultAsync(); // returns only one snippet
        }
    }
}