using DeveloprBud.Data;
using DeveloprBud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;

namespace DeveloprBud.Pages
{
    [Authorize]
    public class CodeSnippetsModel : PageModel
    {
        // DB connection
        private readonly AppDbContext _context;
        public CodeSnippetsModel(AppDbContext context)
        {
            _context = context; // db connection
        }


        // list to hold new code snippets to be displayed on the page
        public IList<CodeSnippet> CodeSnippets { get; set; } = new List<CodeSnippet>(); // initialize to avoid null reference>

        // hold modal form data for the database
        [BindProperty]
        public CodeSnippet NewSnippet { get; set; } = new CodeSnippet(); // initialize to avoid null reference

        // drop down list
        public List<string> CodeLanguages { get; set; } = new()
        {
            "C", "C++", "C#", "Java", "JavaScript", "TypeScript", "Python", "Ruby", "PHP", "Swift", "Kotlin", "Go", "Rust", "Dart", "SQL", "HTML", "CSS", "Bash", "PowerShell"
        };

        // property for search query
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var searchQuery = _context.CodeSnippets
                .Where(s => s.UserId == userId);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                searchQuery = searchQuery.Where(s => s.Title.Contains(Search) || s.Language.Contains(Search));
            }

            CodeSnippets = await searchQuery.ToListAsync();
        }

        public IActionResult OnPostAddSnippet()
        {
            // validate user login
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // if user is not authenticated, redirect to login page
                return RedirectToPage("/Identity/Account/Login");
            }

            NewSnippet.UserId = userId; // associate task with user
            NewSnippet.CreatedDate = DateTime.Now;

            // data validation
            if (!ModelState.IsValid)
            {
                CodeSnippets = _context.CodeSnippets.Where(s => s.UserId == userId).ToList();
                return Page();
            }

            // add and save new code snippet to database
            _context.CodeSnippets.Add(NewSnippet);
            _context.SaveChanges();

            return RedirectToPage(); // refresh the page to show the new task
        }

        // this method deletes the task from the database by finding the task by id
        public async Task<IActionResult> OnPostDeleteSnippetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var snippet = await _context.CodeSnippets.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (snippet != null)
            {
                _context.CodeSnippets.Remove(snippet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/CodeSnippets");
        }
    }
}
