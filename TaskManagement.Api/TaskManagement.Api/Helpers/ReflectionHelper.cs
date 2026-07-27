using System.Reflection;

namespace TaskManagement.Api.Helpers
{
    public class ReflectionHelper
    {
        public static List<string> GetPropertyNames<T>()
        {
            return typeof(T)
                .GetProperties()
                .Select(property => property.Name)
                .ToList();
        }

        public static Dictionary<string, string> GetPropertyTypes<T>()
        {
            return typeof(T)
                .GetProperties()
                .ToDictionary(property => property.Name, property => property.PropertyType.Name);
        }
    }
}
