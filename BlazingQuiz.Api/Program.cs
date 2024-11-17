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
// appsettings.json dosyasýndaki "Quiz" baðlantý dizesini kullanarak veritabaný baðlamýný yapýlandýrdýk.
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<QuizContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Quiz");
    options.UseSqlServer(connectionString);
});

//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

// AuthService sýnýfýný baðýmlýlýk olarak kaydeder ve her istek için yeni bir örnek oluþturur. Bu sayede AuthService sýnýfýný DI(dependency injection) konteyneri üzerinden uygulamanýn herhangi bir yerinde kullanabiliriz.
builder.Services.AddTransient<AuthService>()
    .AddTransient<CategoryService>()
    .AddTransient<QuizService>();

//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

// Kimlik doðrulama iþlevselliði ekledik 
builder.Services.AddAuthentication(options =>
{
    // Varsayýlan doðrulama ve challenge (zorlama) þemalarýný JWT olarak belirliyoruz
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // appsettings.json'dan JWT secret key'i alýyoruz
    var secretKey = builder.Configuration.GetValue<string>("Jwt:Secret");
    // Secret key'i þifreliyoruz (SymmetricSecurityKey ile)
    var symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        // JWT'nin imzasýnýn doðruluðunu kontrol etmek için kullanýlan anahtar
        IssuerSigningKey = symmetricKey,

        // Token'ýn geçerli olduðu Issuer (yayýncý) bilgisi
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),

        // Token'ýn geçerli olduðu Audience (izleyici) bilgisi
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),

        // Issuer'ý doðrulama
        ValidateIssuer = true,

        // Audience'ý doðrulama
        ValidateAudience = true,

        // Token'ýn imzasýný doðrulama
        ValidateIssuerSigningKey = true,
    };
});

//AddAuthorization
builder.Services.AddAuthorization();
//_________________________________________End________________________________________________________________________//


//_________________________________________Start________________________________________________________________________//

builder.Services.AddCors(options =>
{
    // Varsayýlan CORS politikasý ekliyoruz
    options.AddDefaultPolicy(p =>
    {
        // appsettings.json veya appsettings.development.json'dan "AllowedOrigins" ayarýný alýyoruz
        var allowedOriginsStr = builder.Configuration.GetValue<string>("AllowedOrigins");

        // Bu ayarý virgülle ayýrarak bir diziye dönüþtürüyoruz
        var allowedOrigins = allowedOriginsStr.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        // Ýzin verilen domainlerden gelen istekleri kabul ediyoruz
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

//yalnýzca debug modunda çalýþtýrýlmasýný saðlar. Böylece geliþtirme sýrasýnda veritabaný güncellenir, ancak üretim ortamýnda bu iþlem yapýlmaz.
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
//_________________________________________End________________________________________________________________________//
