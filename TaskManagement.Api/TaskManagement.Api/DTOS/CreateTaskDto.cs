using System.ComponentModel.DataAnnotations; //Required, MinLength ve MaxLength gibi validation attribute’larının bulunduğu namespace’tir.

namespace TaskManagement.Api.DTOS
{
    public record CreateTaskDto
    {
        //[Required(ErrorMessage ="Başlık Alanı Zorunludur.")] //Bu özellik Title alanının boş gönderilmemesini sağlar.
        //[MinLength(3, ErrorMessage = "Başlık en az 3 karakter olmalıdır.")] //Başlığın minimum 3 karakter olmasını ister.
        //[MaxLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")] //Başlığın en fazla 100 karakter olmasını sağlar. Bu kontrol önemlidir çünkü client isterse sana binlerce karakter uzunluğunda veri gönderebilir.
        public string Title { get; init; } = string.Empty; //init: nesne oluşturulurken değer atanabilir ama sonradan a=5; gibi değer atanamaz demek.
        //[MaxLength(500, ErrorMessage ="Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; init; } 
        // ? işareti nullable anlamına gelir. yani bu alan boş olabilir demektir.
    }
}
