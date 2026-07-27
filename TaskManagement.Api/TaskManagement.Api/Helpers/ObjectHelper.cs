namespace TaskManagement.Api.Helpers
{
    public class ObjectHelper
    {
        public static bool IsNull<T>(T? value)
        {
            return value is null;
        }

        public static int CountItems<T>(List<T> items)
        {
            return items.Count;
        }
    }
}
