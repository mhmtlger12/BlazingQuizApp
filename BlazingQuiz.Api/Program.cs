using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// appsettings.json dosyas�ndaki "Quiz" ba�lant� dizesini kullanarak veritaban� ba�lam�n� yap�land�rd�k.
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<QuizContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Quiz");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

//yaln�zca debug modunda �al��t�r�lmas�n� sa�lar. B�ylece geli�tirme s�ras�nda veritaban� g�ncellenir, ancak �retim ortam�nda bu i�lem yap�lmaz.
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

//Debug modundayken uodate-database yapmak yerine, yeni migration varsa  bunu api  aya�a kald�r�rken kendisi update database yap�yor. 
static void AppleyDbMigrations(IServiceProvider sp)
{
    var scope = sp.CreateScope(); // Ba��ml�l�k ��z�mleme i�in yeni bir kapsam (scope) olu�turuyoruz.
    var context = scope.ServiceProvider.GetRequiredService<QuizContext>(); // Yeni olu�turulan kapsam �zerinden QuizContext �rne�ini al�yoruz.
    if (context.Database.GetPendingMigrations().Any()) // Bekleyen migration olup olmad���n� kontrol ediyoruz.
    {
        context.Database.Migrate(); // Bekleyen migrationlar� uygular.
    }
}

