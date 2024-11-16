using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingQuiz.Shared.DTOs
{
    //API'den dönen başarı ve hata durumlarını standart bir formatta temsil etmek için
    public record QuizApiResponse(bool IsSuccess ,string? ErrorMessage)
    {
        public static QuizApiResponse Success() => new(true, null);
        public static QuizApiResponse Fail(string errorMessage) => new(false, errorMessage);

    }
}
