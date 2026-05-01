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
    public class TasksModel : PageModel
    {
        // DB connection
        private readonly AppDbContext _context;
        public TasksModel(AppDbContext context)
        {
            _context = context; // db connection
        }



        public IList<TaskItem> TaskItems { get; set; } = new List<TaskItem>(); // hold task items
        public List<string> PriorityLevel { get; set; } = new()
        {
            "Low", "Medium", "High" // populate drop down
        };

        // hold modal form data for the database - property
        [BindProperty]
        public TaskItem NewTask { get; set; } = new TaskItem(); // initialize to avoid null reference

        // property for search query
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        // task filter property
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }




        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var searchQuery = _context.TaskItems
                .Where(t => t.UserId == userId && t.Status != "Completed");

            // perform search
            if (!string.IsNullOrWhiteSpace(Search))
            {
                searchQuery = searchQuery.Where(t => t.Title.Contains(Search) || t.Tag.Contains(Search));
            }

            // perform priority filter
            if (!string.IsNullOrWhiteSpace(Filter) && Filter != "All")
            {
                searchQuery = searchQuery.Where(t => t.Priority == Filter);
            }

            TaskItems = await searchQuery.ToListAsync();
        }


        // this method adds a new task to the database
        public async Task<IActionResult> OnPostAddTaskAsync()
        {
            // validate user login
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // if user is not authenticated, redirect to login page
                return RedirectToPage("/Identity/Account/Login");
            }

            NewTask.UserId = userId; // associate task with user
            NewTask.Status = "Not Completed"; // default task status upon creation
            NewTask.CreatedDate = DateTime.Now;

            // data validation - server side
            if (!ModelState.IsValid)
            {
                TaskItems = await _context.TaskItems.Where(t => t.UserId == userId && t.Status != "Completed").ToListAsync();
                return Page();
            }

            _context.TaskItems.Add(NewTask);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        // This method marks the task as completed and updates the completion date automatically
        public async Task<IActionResult> OnPostCompleteTaskAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task != null)
            {
                task.Status = "Completed";
                task.CompletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }


        // this method deletes the task from the database by finding the task by id
        public async Task<IActionResult> OnPostDeleteTaskAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task != null)
            {
                _context.TaskItems.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}

