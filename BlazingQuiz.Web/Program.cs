using BlazingQuiz.Web;
using BlazingQuiz.Web.Apis;
using BlazingQuiz.Web.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");






//_________________________________________Start:Auth________________________________________________________________________//

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<QuizAuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp=>sp.GetRequiredService<QuizAuthStateProvider>());
builder.Services.AddAuthorizationCore();
//_________________________________________End________________________________________________________________________//



//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }); bunu kullanmýyoruz artýk 

ConfigureRefit(builder.Services);
await builder.Build().RunAsync();

//_________________________________________Start:Refit________________________________________________________________________//
// Refit ile API istemcisi yapýlandýrýyoruz
static void ConfigureRefit(IServiceCollection services)
{
    // API'nin temel URL'sini tanýmlýyoruz
    const string ApiBaseUrl = "https://localhost:7192";

    // RefitClient ekliyoruz ve IAuthApi arayüzünü kullanarak HTTP istemcisini yapýlandýrýyoruz
    services.AddRefitClient<IAuthApi>()
        .ConfigureHttpClient(httpClient =>
            // HTTP istemcisinin temel adresini belirliyoruz
            httpClient.BaseAddress = new Uri(ApiBaseUrl));
}
//_________________________________________End________________________________________________________________________//
