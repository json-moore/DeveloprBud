namespace DeveloprBud.Helpers
{
    public static class CodeLanguageHelper
    {
        // Code language list for code snippet creation and syntax highlighting
        public static List<string> CodeLanguages { get; } = new()
        {
            "plaintext",
            "csharp",
            "c_cpp",
            "java",
            "javascript",
            "typescript",
            "python",
            "ruby",
            "php",
            "swift",
            "kotlin",
            "golang",
            "rust",
            "dart",
            "sql",
            "html",
            "css",
            "sh",
            "powershell"
        };
    }
}
