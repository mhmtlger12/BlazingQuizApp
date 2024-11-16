using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Services
{
    public class CategoryService
    {
        private readonly QuizContext _context;
        public CategoryService(QuizContext context)
        {
            _context = context;
        }

        public async Task<QuizApiResponse> SaveCategoryAsync(CategoryDto dto)
        {
            if (await _context.Categories
                .AsNoTracking()
                .AnyAsync(c=>c.Name==dto.Name && c.Id!=dto.Id))
            {
                //Aynı isimli kategori zaten mevcut
                return QuizApiResponse.Fail("Aynı isimli kategori zaten mevcut");
            }
            if (dto.Id==0)
            {
                //yeni kategori ekleme
                var category = new Category
                {
                    Name = dto.Name

                };
                _context.Categories.Add(category);  
            }
            else
            {
                //Update Category
                var dbCategory= await _context.Categories
                    .FirstOrDefaultAsync(c=>c.Id==dto.Id);
                if(dbCategory==null)
                {

                    //Kategori mevcut değil, hata atılıyor veya hata yanıtı gönderiliyor
                    return QuizApiResponse.Fail("Kategori mevcut değil");
                }
                dbCategory.Name = dto.Name;
                _context.Categories.Update(dbCategory);
            }
           await _context.SaveChangesAsync();
            return QuizApiResponse.Success();
                
        }
        public async Task<CategoryDto[]> GetCategoriesAsync() =>
            await _context.Categories.AsNoTracking()
            .Select(c => new CategoryDto
            {
                Name=c.Name,
                Id=c.Id
            })
            .ToArrayAsync();
        }
    }
