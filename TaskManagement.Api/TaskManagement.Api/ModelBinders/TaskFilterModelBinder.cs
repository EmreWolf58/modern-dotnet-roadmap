using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using TaskManagement.Api.Model;

namespace TaskManagement.Api.ModelBinders
{
    public class TaskFilterModelBinder: IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;

            var valueResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueResult.FirstValue;
            var parts = value?.Split('|');

            if (parts == null || parts.Length !=2)
            {
                bindingContext.ModelState.AddModelError(modelName, "Filter formatı 'status|yyyy-MM-dd' olmalıdır.");
                return Task.CompletedTask;
            }

            if (!DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                bindingContext.ModelState.AddModelError(modelName, "Tarih yyyy-MM-dd formatında olmalıdır.");
                return Task.CompletedTask;
            }

            var filter = new TaskFilter
            {
                Status = parts[0],
                Date = date
            };
            bindingContext.Result = ModelBindingResult.Success(filter);
            return Task.CompletedTask;
        }
    }
}
