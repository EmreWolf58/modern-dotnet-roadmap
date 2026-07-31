using FluentValidation;
using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Validators
{
    public class TaskQueryValidator: AbstractValidator<TaskQuery>
    {
        private static readonly string[] AllowedSortFields =
        {
            "id",
            "title",
            "createdDate"
        };

        public TaskQueryValidator()
        {
            RuleFor(query => query.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

            RuleFor(query => query.Search)
                .MaximumLength(100)
                .WithMessage("Arama metni en fazla 100 karakter olabilir.");

            RuleFor(query => query.SortBy)
                .Must(BeAValidSortField)
                .WithMessage(
                    "SortBy alanı id, title veya createdDate olmalıdır."
                );
        }
        private static bool BeAValidSortField(string sortBy)
        {
            return AllowedSortFields.Contains(
                sortBy,
                StringComparer.OrdinalIgnoreCase
            );
        }
    }
}
