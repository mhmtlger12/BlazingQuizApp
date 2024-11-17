﻿using BlazingQuiz.Api.Data; // Veritabanı bağlamını ve varlıkları içeren namespace
using BlazingQuiz.Api.Data.Entities; // Sınav ve sorularla ilgili varlık sınıflarını içerir
using BlazingQuiz.Shared.DTOs; // Veri transfer nesneleri için namespace
using Microsoft.EntityFrameworkCore; // Entity Framework Core işlevlerini kullanmak için

namespace BlazingQuiz.Api.Services
{
    public class QuizService
    {
        private readonly QuizContext _context; // Veritabanı bağlamını temsil eden alan

        public QuizService(QuizContext context)
        {
            _context = context; // Veritabanı bağlamını kurucu metottan alır ve sınıf alanına atar
        }

        /// <summary>
        /// Yeni bir sınav kaydetme veya mevcut bir sınavı güncelleme işlemi yapar.
        /// </summary>
        /// <param name="dto">Sınav verilerini içeren DTO</param>
        /// <returns>Kaydetme işleminin sonucu</returns>
        public async Task<QuizApiResponse> SaveQuizAsync(QuizSaveDto dto)
        {
            // Gelen DTO'dan soruları oluşturur
            var questions = dto.Questions.Select(q => new Question
            {
                Id = q.Id, // Sorunun ID'si (varsa)
                Text = q.Text, // Sorunun metni
                Options = q.Options.Select(p => new Option // Sorunun seçeneklerini oluşturur
                {
                    Id = p.Id, // Seçeneğin ID'si (varsa)
                    Text = p.Text, // Seçeneğin metni
                    IsCorrect = p.IsCorrect // Seçeneğin doğru olup olmadığını belirten alan
                }).ToArray() // Seçenekleri bir diziye dönüştürür
            }).ToArray(); // Soruları bir diziye dönüştürür

            // Eğer DTO'nun ID'si boşsa (yeni sınav oluşturuluyor)
            if (dto.Id == Guid.Empty)
            {
                // Yeni bir sınav oluşturur
                var quiz = new Quiz
                {
                    CategoryId = dto.CategoryId, // Kategori ID'si
                    IsActive = dto.IsActive, // Sınavın aktif olup olmadığı bilgisi
                    Name = dto.Name, // Sınavın adı
                    TimeInMinutes = dto.TimeInMinutes, // Sınav süresi
                    TotalQuestions = dto.TotalQuestions, // Toplam soru sayısı
                    Questions = questions // Sınavın soruları
                };
                _context.Quizzes.Add(quiz); // Yeni sınavı veritabanına ekler
            }
            else
            {
                // Mevcut bir sınavı günceller
                var dbQuiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == dto.Id); // Sınavı veritabanından getirir
                if (dbQuiz == null)
                {
                    return QuizApiResponse.Fail("Sınav mevcut değil"); // Sınav bulunamazsa hata döner
                }

                // Mevcut sınavın bilgilerini günceller
                dbQuiz.CategoryId = dto.CategoryId;
                dbQuiz.IsActive = dto.IsActive;
                dbQuiz.Name = dto.Name;
                dbQuiz.TotalQuestions = dto.TotalQuestions;
                dbQuiz.TimeInMinutes = dto.TimeInMinutes;
                dbQuiz.Questions = questions;

                _context.Quizzes.Update(dbQuiz); // Güncellenen sınavı veritabanına yansıtır
            }

            try
            {
                await _context.SaveChangesAsync(); // Veritabanı değişikliklerini kaydeder
                return QuizApiResponse.Success(); // Başarılı yanıt döner
            }
            catch (Exception ex)
            {
                return QuizApiResponse.Fail(ex.Message); // Hata durumunda hata mesajını döner
            }
        }
    }
}
