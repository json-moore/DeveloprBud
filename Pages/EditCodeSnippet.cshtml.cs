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
    public class EditCodeSnippetModel : PageModel
    {
        // DB connection
        private readonly AppDbContext _context;
        public EditCodeSnippetModel(AppDbContext context)
        {
            _context = context; // db connection
        }



        [BindProperty]
        public CodeSnippet SingleSnippet { get; set; } = new CodeSnippet();



        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var codeSnippet = await _context.CodeSnippets
                .FirstOrDefaultAsync(s => s.Id == id &&
                                          s.UserId == userId);

            if (codeSnippet == null)
            {
                return RedirectToPage("/CodeSnippets");
            }

            SingleSnippet = codeSnippet;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var storedDBCode = await _context.CodeSnippets
                .FirstOrDefaultAsync(s => s.Id == SingleSnippet.Id &&
                                          s.UserId == userId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (storedDBCode == null)
            {
                return RedirectToPage("/CodeSnippets");
            }

            storedDBCode.Title = SingleSnippet.Title;
            storedDBCode.Code = SingleSnippet.Code;
            storedDBCode.Language = SingleSnippet.Language;
            storedDBCode.Notes = SingleSnippet.Notes;

            await _context.SaveChangesAsync();

            return RedirectToPage("/CodeSnippets");
        }
    }
}
