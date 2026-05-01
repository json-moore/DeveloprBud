using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DeveloprBud.Models;

namespace DeveloprBud.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
    }
}