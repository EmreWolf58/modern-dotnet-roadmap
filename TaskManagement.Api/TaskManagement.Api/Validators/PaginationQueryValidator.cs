using FluentValidation;
using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Validators
{
    public class PaginationQueryValidator: AbstractValidator<PaginationQuery>
    {
        public PaginationQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");
        }
    }
}
