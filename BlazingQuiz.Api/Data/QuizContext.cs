using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Data
{
    public class QuizContext : DbContext
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public QuizContext(DbContextOptions<QuizContext> options,IPasswordHasher<User> passwordHasher) : base(options)
        {
            _passwordHasher = passwordHasher;
        }
        public DbSet<Category>  Categories { get; set; }
        public DbSet<Option>  Options { get; set; }
        public DbSet<Question>  Questions { get; set; }
        public DbSet<Quiz>  Quizzes { get; set; }
        public DbSet<StudentQuiz> StudentQuizzes { get; set; }
        public DbSet<User>  Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Yönetici kullanıcımız. DB oluşuruken vaersayılan olarak bir yönetici atıyoruz.
            var adminUser = new User
            {
                Name = "Mehmet Güler",
                Email = "mhmtgler12@gmail.com",
                Id = 1,
                Phone = "5367017748",
                Role = nameof(UserRole.Admin),
                IsApproved = true
               

            };
            adminUser.PassworsdHash = _passwordHasher.HashPassword(adminUser, "qwerty123");

            //uygulama ilk kez çalıştığında veritabanı oluşturulurken, belirttiğimiz adminUser verisi otomatik olarak veritabanına ekleniyor.
            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
