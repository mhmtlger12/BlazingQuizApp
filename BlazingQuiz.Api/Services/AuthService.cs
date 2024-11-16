using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazingQuiz.Api.Services
{
    // AuthService sınıfı, kullanıcı doğrulama ve JWT token üretme işlemlerini içerir.
    public class AuthService
    {
        private readonly QuizContext _quizContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        // AuthService sınıfının constructor'ı, QuizContext ve IPasswordHasher<User> bağımlılıklarını alır.
        // Bu bağımlılıklar sınıfın işlevselliği için gereklidir.
        public AuthService(QuizContext quizContext, IPasswordHasher<User> passwordHasher,IConfiguration configuration)
        {
            _quizContext = quizContext; // Veritabanı bağlamı (DbContext) alınıyor.
            _passwordHasher = passwordHasher; // Şifre hashleme servisi alınıyor.
            _configuration = configuration;
        }

        // Login işlemi için kullanılan metod.
        // Bu metod, kullanıcı adı ve şifresini doğrular ve geçerli bir kullanıcı ise JWT token üretir.
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Kullanıcının var olup olmadığını kontrol ediyoruz.
            var user = await _quizContext.Users
                .AsNoTracking() // Performans iyileştirmesi için takip edilmeyen sorgu.
                .FirstOrDefaultAsync(u => u.Email == dto.UserName); // Kullanıcı adı (Email) ile sorgu yapılıyor.

            if (user == null)
            {
                // Eğer kullanıcı bulunamazsa, işlemi sonlandırıyoruz. 
                return new AuthResponseDto(default, "Geçersiz Kullanıcı Adı");
            }

            // Verilen şifrenin doğru olup olmadığını kontrol ediyoruz.
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PassworsdHash, dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                // Şifre yanlış ise, işlem sonlandırılır. 
                return new AuthResponseDto (default, "Geçersiz Şifre");
            }

            // Kullanıcı doğrulandı, şimdi JWT token oluşturuluyor.
            var jwt = GenerateJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id, user.Name, user.Role, jwt);
            return new AuthResponseDto(loggedInUser);
        }

        // JWT token'ı üreten metod.
        private  string GenerateJwtToken(User user)
        {
            // JWT'nin içine dahil edilecek claim'ler (kullanıcı bilgileri).
            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcı ID'si
                new Claim(ClaimTypes.Name, user.Name), // Kullanıcı adı
                new Claim(ClaimTypes.Role, user.Role) // Kullanıcı rolü
            ];

            // Secret key, genellikle appsettings.json'dan alınır.
            var secretKey =_configuration.GetValue<string>("Jwt:Secret"); // Burada secret key appsettings'den alınabilir.
            var symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)); // Anahtar şifreleniyor.

            // SigningCredentials ile imzalama işlemi yapılır. 
            var signingCred = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            // JWT token'ın oluşturulması.
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer"), // Issuer (kim tarafından oluşturulduğu bilgisi) belirtilmeli.
                audience: _configuration.GetValue<string>("Jwt:Audience"), // Audience (kimin için oluşturulduğu) belirtilmeli.
                claims: claims, // Kullanıcı bilgilerini içeren claim'ler.
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireInMinutes")), // Token geçerlilik süresi (400 dakika).
                signingCredentials: signingCred // İmzalama bilgisi.
            );

            // JWT token string olarak döndürülür.
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token; // Üretilen token'ı döndürür.
        }
    }
}
