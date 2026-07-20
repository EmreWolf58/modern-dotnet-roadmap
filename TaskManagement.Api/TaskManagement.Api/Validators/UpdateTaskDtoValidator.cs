using FluentValidation;
using TaskManagement.Api.DTOS;

namespace TaskManagement.Api.Validators
{
    public class UpdateTaskDtoValidator: AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Başlık Zorunludur.")
                .MinimumLength(3)
                .WithMessage("Başlık en az 3 karakter olmalıdır.")
                .MaximumLength(100)
                .WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Açıklama en fazla 500 karakter olabilir.");
        }
    }
}
