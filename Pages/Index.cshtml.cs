using DeveloprBud.Data;
using DeveloprBud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DeveloprBud.APIs.WeatherAPI.Services;
using DeveloprBud.APIs.WeatherAPI.Models;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace DeveloprBud.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        // database connection
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context, WeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        // inject weather service
        private readonly WeatherService _weatherService;

        public int TotalTasksOpen { get; set; } // total tasks open - displayed in dashboard
        public int TotalExistingSnippets { get; set; } // total code snippets - displayed in dashboard
        public TaskItem? LongestTaskOpen { get; set; } // longest task open - displayed in dashboard
        public int? LongestTaskDayCount { get; set; } // longest task open day count - displayed in dashboard
        public int TasksCompletedToday { get; set; } // tasks completed today - displayed in dashboard
        public int TasksCompletedThisWeek { get; set; } // tasks completed this week - displayed in dashboard
        public int TasksCompletedThisMonth { get; set; } // tasks completed this month - displayed in dashboard
        public CodeSnippet? LastSnippetSaved { get; set; } // last snippet saved - displayed in dashboard
        public WeatherResponse? Weather { get; set; } // current weather - displayed in dashboard

        public async Task OnGetAsync(string zip)
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

            // Only call the API **IF** the the user enters a zip code and searches for their local weather
            if (!string.IsNullOrWhiteSpace(zip))
            {
                Weather = await _weatherService.GetCurrentWeatherAsync(zip);

                // convert weather response json object to text for browser session storage
                HttpContext.Session.SetString(
                    "WeatherData",
                    JsonSerializer.Serialize(Weather));

            }
            else
            {
                var cachedWeather = HttpContext.Session.GetString("WeatherData");


                if (!string.IsNullOrEmpty(cachedWeather))
                {
                    Weather = JsonSerializer.Deserialize<WeatherResponse>(cachedWeather);
                }
            }
        }

        public string GetPrismLanguage(string? language)
        {
            return language switch
            {
                "html" => "markup", // Prism "markup"
                "xml" => "markup",
                "c_cpp" => "cpp",   // Prism "cpp"
                null => "plaintext",
                "" => "plaintext",
                _ => language
            };
        }
    }
}