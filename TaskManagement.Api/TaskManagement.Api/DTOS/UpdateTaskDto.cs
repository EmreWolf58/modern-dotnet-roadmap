using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.DTOS
{
    public record UpdateTaskDto
    {
        //[Required(ErrorMessage = "Başlık Alanı Zorunludur.")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Başlık 3 ile 100 karakter arasında olmalıdır.")]
        public string Title { get; init; } = string.Empty;
        //[StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; init; }

        public bool IsCompleted { get; init; }
    }
}
