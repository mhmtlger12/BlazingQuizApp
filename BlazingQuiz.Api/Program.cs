using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Api.Endpoints;
using BlazingQuiz.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//_________________________________________Start________________________________________________________________________//
// appsettings.json dosyas�ndaki "Quiz" ba�lant� dizesini kullanarak veritaban� ba�lam�n� yap�land�rd�k.
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<QuizContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Quiz");
    options.UseSqlServer(connectionString);
});

//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

// AuthService s�n�f�n� ba��ml�l�k olarak kaydeder ve her istek i�in yeni bir �rnek olu�turur. Bu sayede AuthService s�n�f�n� DI(dependency injection) konteyneri �zerinden uygulaman�n herhangi bir yerinde kullanabiliriz.
builder.Services.AddTransient<AuthService>()
    .AddTransient<CategoryService>()
    .AddTransient<QuizService>();

//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

// Kimlik do�rulama i�levselli�i ekledik 
builder.Services.AddAuthentication(options =>
{
    // Varsay�lan do�rulama ve challenge (zorlama) �emalar�n� JWT olarak belirliyoruz
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // appsettings.json'dan JWT secret key'i al�yoruz
    var secretKey = builder.Configuration.GetValue<string>("Jwt:Secret");
    // Secret key'i �ifreliyoruz (SymmetricSecurityKey ile)
    var symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        // JWT'nin imzas�n�n do�rulu�unu kontrol etmek i�in kullan�lan anahtar
        IssuerSigningKey = symmetricKey,

        // Token'�n ge�erli oldu�u Issuer (yay�nc�) bilgisi
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),

        // Token'�n ge�erli oldu�u Audience (izleyici) bilgisi
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),

        // Issuer'� do�rulama
        ValidateIssuer = true,

        // Audience'� do�rulama
        ValidateAudience = true,

        // Token'�n imzas�n� do�rulama
        ValidateIssuerSigningKey = true,
    };
});

//AddAuthorization
builder.Services.AddAuthorization();
//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

builder.Services.AddCors(options =>
{
    // Varsay�lan CORS politikas� ekliyoruz
    options.AddDefaultPolicy(p =>
    {
        // appsettings.json veya appsettings.development.json'dan "AllowedOrigins" ayar�n� al�yoruz
        var allowedOriginsStr = builder.Configuration.GetValue<string>("AllowedOrigins");

        // Bu ayar� virg�lle ay�rarak bir diziye d�n��t�r�yoruz
        var allowedOrigins = allowedOriginsStr.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        // �zin verilen domainlerden gelen istekleri kabul ediyoruz
        p.WithOrigins(allowedOrigins)
         // Herhangi bir header'a izin veriyoruz
         .AllowAnyHeader()
         // Herhangi bir HTTP metoduna (GET, POST, PUT, DELETE vb.) izin veriyoruz
         .AllowAnyMethod();
    });
});

//_________________________________________End________________________________________________________________________//



var app = builder.Build();

//_________________________________________Start________________________________________________________________________//

//yaln�zca debug modunda �al��t�r�lmas�n� sa�lar. B�ylece geli�tirme s�ras�nda veritaban� g�ncellenir, ancak �retim ortam�nda bu i�lem yap�lmaz.
#if DEBUG
AppleyDbMigrations(app.Services);
#endif
//_________________________________________End________________________________________________________________________//

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Endpoints dahil ediyoruz.BlazingQuiz.Api\Endpoints\
app.MapAuthEndpoints()
    .MapCategoryEndPoints();
app.UseAuthentication()
    .UseAuthorization();
app.UseCors();
app.Run();


//_________________________________________Start________________________________________________________________________//
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
//_________________________________________End________________________________________________________________________//
