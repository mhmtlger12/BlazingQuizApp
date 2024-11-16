using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared.DTOs;

namespace BlazingQuiz.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)

        {
            // /api/auth/login endpoint'ini tanımlıyoruz ve LoginDto ile gelen veriyi AuthService'e gönderiyoruz
            app.MapPost("/api/auth/login", async (LoginDto dto, AuthService authService) =>
                Results.Ok(await authService.LoginAsync(dto))); // Login işlemi başarılı ise 200 OK döndürülür

            return app; // Endpoints'e yapılan eklemeler sonrası route builder'ı geri döndürüyoruz
        }
    }
}
