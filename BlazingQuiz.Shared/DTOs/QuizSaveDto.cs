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
        [Required,MaxLength(100)]
        public string Name { get; set; }
        [Range(1,int.MaxValue,ErrorMessage="Kategori Alanı Zorunludur")]
        public int CategoryId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli soru sayısı belirtin.")]
        public int TotalQuestions { get; set; }
        //2 SAAT YAPTIK
        [Range(1, 120, ErrorMessage = "lütfen geçerli sayıyı dakika cinsinden belirtin")]
        public int TimeInMinutes { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto> Questions { get; set; } = [];
    }
}
