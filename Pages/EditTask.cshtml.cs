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
    public class EditTaskModel : PageModel
    {
        // database connection
        private readonly AppDbContext _context;
        public EditTaskModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskItem TaskItem { get; set; } = new TaskItem();


        // load the task from DB
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); ;

            if (task == null)
            {
                return RedirectToPage("/Tasks");
            }

            TaskItem = task;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var storedDBTask = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == TaskItem.Id &&
                                          t.UserId == userId);

            if (storedDBTask == null)
            {
                return RedirectToPage("/Tasks");
            }

            storedDBTask.Title = TaskItem.Title;
            storedDBTask.Description = TaskItem.Description;
            storedDBTask.Priority = TaskItem.Priority;
            storedDBTask.Tag = TaskItem.Tag;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Tasks");
        }
    }
}
