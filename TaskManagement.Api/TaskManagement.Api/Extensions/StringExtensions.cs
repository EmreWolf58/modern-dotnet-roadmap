namespace TaskManagement.Api.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string? value) //Buradaki this, metodun string değişkenleri üzerinden çağrılmasını sağlar.
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string NormalizeText(this string? value)
        {
            return value.Trim();
        }
    }
}
