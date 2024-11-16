using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// appsettings.json dosyasýndaki "Quiz" baðlantý dizesini kullanarak veritabaný baðlamýný yapýlandýrdýk.
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<QuizContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Quiz");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

//yalnýzca debug modunda çalýþtýrýlmasýný saðlar. Böylece geliþtirme sýrasýnda veritabaný güncellenir, ancak üretim ortamýnda bu iþlem yapýlmaz.
#if DEBUG
AppleyDbMigrations(app.Services);
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

//Debug modundayken uodate-database yapmak yerine, yeni migration varsa  bunu api  ayaða kaldýrýrken kendisi update database yapýyor. 
static void AppleyDbMigrations(IServiceProvider sp)
{
    var scope = sp.CreateScope(); // Baðýmlýlýk çözümleme için yeni bir kapsam (scope) oluþturuyoruz.
    var context = scope.ServiceProvider.GetRequiredService<QuizContext>(); // Yeni oluþturulan kapsam üzerinden QuizContext örneðini alýyoruz.
    if (context.Database.GetPendingMigrations().Any()) // Bekleyen migration olup olmadýðýný kontrol ediyoruz.
    {
        context.Database.Migrate(); // Bekleyen migrationlarý uygular.
    }
}

