using System.ComponentModel.DataAnnotations;

namespace BlazingQuiz.Shared.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        [Required,MaxLength(350)]
        public string Text { get; set; }

        public List<OptionDto> Options { get; set; } = [];
    }
}
