using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;

namespace BlazingQuiz.Api.Endpoints
{
    public static class CategoryEndpoints
    {
        //Bu kod parçasında, bir API endpoint grubu tanımlıyoruz. Blazor uygulamasında kategorilerle ilgili işlemleri yönetmek için bir CategoryEndpoints sınıfı oluşturuyoruz. Kısaca, kategorileri listelemek ve kaydetmek için gereken HTTP isteklerini bu sınıfta tanımlıyoruz.


        public static IEndpointRouteBuilder MapCategoryEndPoints(this IEndpointRouteBuilder app)
        {
            var categoryGroup = app.MapGroup("/api/categories")
                .RequireAuthorization();
            categoryGroup.MapGet("", async (CategoryService categoryService) =>
            Results.Ok(await categoryService.GetCategoriesAsync()));

            //Bu, sadece admin rolüne sahip kullanıcıların belirli bir endpoint'e (bu durumda POST /api/categories) erişmesini sağlar.
            categoryGroup.MapPost("",async(CategoryDto dto,CategoryService categoryService)=>
            Results.Ok(await categoryService.SaveCategoryAsync(dto))).RequireAuthorization(p=>p.RequireRole(nameof(UserRole.Admin)));
            return app;
        }
    }
}
