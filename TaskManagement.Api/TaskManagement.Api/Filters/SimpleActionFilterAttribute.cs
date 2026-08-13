using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class SimpleActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("SimpleActionFilter: Action başladı.");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("SimpleActionFilter: Action tamamlandı.");
        }
    }
}
