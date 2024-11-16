using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazingQuiz.Shared.DTOs
{
    // AuthResponseDto, kimlik doğrulama işlemi sonucu döndürülecek yanıtı temsil eder.
    // Bu sınıf, token ve hata mesajı (opsiyonel) içerir.
    public record AuthResponseDto(string Token, string? ErrorMessage = null)
    {
        // HasError özelliği, hata mesajı olup olmadığını kontrol eder.
        [JsonIgnore]
        public bool HasError => ErrorMessage != null;
    }
}
