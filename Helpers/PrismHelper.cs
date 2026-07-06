namespace DeveloprBud.Helpers
{
    public static class PrismHelper
    {
        public static string GetPrismLanguage(string? language)
        {
            return language switch
            {
                "html" => "markup", // Prism uses "markup"
                "xml" => "markup",
                "c_cpp" => "cpp",   // Prism uses "cpp"
                null => "plaintext",
                "" => "plaintext",
                _ => language
            };
        }
    }
}
