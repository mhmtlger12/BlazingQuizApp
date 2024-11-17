using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingQuiz.Shared.DTOs
{
    public class QuizSaveDto
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Kategori Alanı Zorunludur")]
        public int CategoryId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli soru sayısı belirtin.")]
        public int TotalQuestions { get; set; }
        //2 SAAT YAPTIK
        [Range(1, 120, ErrorMessage = "lütfen geçerli sayıyı dakika cinsinden belirtin")]
        public int TimeInMinutes { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto> Questions { get; set; } = [];

        public string? Validate()
        {
            if (TotalQuestions != Questions.Count)
                return "Soru sayısı Toplam Soru ile eşleşmiyor";
            if (Questions.Any(q => string.IsNullOrWhiteSpace(q.Text)))
                return "Soru metni zorunludur";
            if (Questions.Any(q => q.Options.Count< 2))
                return "Her soru için en az 2 Seçenek gereklidir";

            if (Questions.Any(q => !q.Options.Any(o => o.IsCorrect)))
                return "Tüm seçeneklerde doğru yanıt işaretlenmiş olmalıdır";


            return null;
        }
    }
}
