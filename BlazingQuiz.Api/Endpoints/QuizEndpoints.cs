using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared.DTOs; // DTO'lar (Data Transfer Object) için kullanılan namespace.

namespace BlazingQuiz.Api.Endpoints
{
    public static class QuizEndpoints
    {
        // Bu yöntem, uygulamanın endpoint rotalarını tanımlamak için genişletilebilir bir yapı sağlar.
        public static  IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
        {
            // "/api/quizes" URL'si altında çalışan, yetkilendirme gerektiren bir endpoint grubu oluşturuyoruz.
            var quizGroup = app.MapGroup("/api/quizes").RequireAuthorization();

            // Bir POST endpoint tanımlıyoruz. Yeni sınav eklemek veya mevcut sınavı güncellemek için kullanılıyor.
            quizGroup.MapPost("", async (QuizSaveDto dto, QuizService service) =>
            {
                // İlk olarak, gelen DTO'daki sorular listesinin boş olup olmadığını kontrol ediyoruz.
                if (dto.Questions.Count == 0)
                    return Results.BadRequest("Lütfen soruları belirtin"); // Eğer boşsa, 400 BadRequest ile hata döndür.

                // DTO'daki toplam soru sayısının, gönderilen soruların sayısıyla eşleşip eşleşmediğini kontrol ediyoruz.
                if (dto.Questions.Count != dto.TotalQuestions)
                    return Results.BadRequest("Toplam soru sayısı sağlanan sorularla eşleşmiyor"); // Eşleşmiyorsa hata döndür.

                // Sınav kaydetme işlemi için `QuizService`'in `SaveQuizAsync` metodunu çağırıyoruz.
                // Bu metod, sınavın yeni mi yoksa mevcut bir sınav mı olduğuna göre kaydetme veya güncelleme yapar.
                return Results.Ok(await service.SaveQuizAsync(dto)); // İşlem başarılıysa 200 OK ile sonuç döner.
            });
            quizGroup.MapGet("", async (QuizService service) =>
               Results.Ok(await service.GetQuizesAsync()));

            quizGroup.MapGet("{quizId:guid}/questions", async (Guid quizId, QuizService service) =>
                {
                    return Results.Ok(await service.GetQuizQuestionsAsync(quizId));
                });
            quizGroup.MapGet("{quizId:guid}", async (Guid quizId, QuizService service) =>
                    Results.Ok(await service.GetQuizToEditAsync(quizId)));

            // Bu endpoint grubunu tanımlayan app nesnesini döndürüyoruz.
            return app;
        }
    }
}
