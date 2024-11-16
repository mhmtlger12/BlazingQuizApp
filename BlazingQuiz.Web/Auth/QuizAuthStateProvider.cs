using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace BlazingQuiz.Web.Auth
{
    public class QuizAuthStateProvider : AuthenticationStateProvider
    {
        private const string AuthType = "quiz-auth";
        private const string UserDataKey = "udata";
        private Task<AuthenticationState> _authStateTask;

        private readonly IJSRuntime _jsonRuntime;
        public QuizAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsonRuntime = jsRuntime;
            SetAuthStateTask();
        }

        // Bu metod, kimlik doğrulama durumunu almak için asenkron olarak çağrılır.
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>

           // Önceden oluşturulan _authStateTask'i döndürerek kullanıcı kimlik doğrulama durumunu sağlar.
           _authStateTask;

        public LoggedInUser User { get; private set; }
        public bool IsLoggedIn => User?.Id > 0;

        public IJSRuntime JsRuntime { get; }

        public async Task SetLoginAsync(LoggedInUser user)
        {
            User = user;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jsonRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, user.ToJson());
        }

        public async Task SetLogoutAsync()
        {
            User = null;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jsonRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);
        }
        // Kullanıcının oturum açma durumunu uygulama başlangıcında yüklemek için kullanılır.
        public bool IsInitializing { get; private set; } = true; // Başlangıçta işlem devam ediyor olarak işaretlenir.

        public async Task InitializeAsync()
        {
            try
            {
                // Tarayıcının localStorage'ından kullanıcı verilerini alır.
                var udata = await _jsonRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
                if (string.IsNullOrWhiteSpace(udata))
                {
                    // Kullanıcı verisi varsa işlem sona erer.
                    return;
                }

                // Veriyi LoggedInUser nesnesine dönüştürür.
                var user = LoggedInUser.LoadFrom(udata);
                if (user == null || user.Id == 0)
                {
                    // Kullanıcı verisi geçerli değilse işlem sona erer.
                    return;
                }

                // Kullanıcı oturum açmışsa oturum bilgilerini ayarlar.
                await SetLoginAsync(user);
            }
            finally
            {
                // İşlem tamamlandığında "başlangıç işlemi devam ediyor" durumu sona erdirilir.
                IsInitializing = false;
            }
        }



        private void SetAuthStateTask()
        {
            if (IsLoggedIn)
            {
                var identity = new ClaimsIdentity(User.ToClaims(), AuthType);
                var principal = new ClaimsPrincipal(identity);
                // Yeni bir AuthenticationState nesnesi oluşturuluyor, bu nesne kullanıcı kimlik bilgilerini temsil eder.
                var authState = new AuthenticationState(principal);

                // Asenkron bir görev olarak authState'i sakla.
                _authStateTask = Task.FromResult(authState);
            }
            else
            {
                // Kullanıcı kimlik bilgilerini temsil etmek için bir ClaimsIdentity nesnesi oluşturuluyor.
                var identity = new ClaimsIdentity();
                // Kullanıcıyı temsil eden bir ClaimsPrincipal nesnesi oluşturuluyor. Şu anda kimlik doğrulama bilgisi içermiyor.
                var principal = new ClaimsPrincipal(identity);
                // Yeni bir AuthenticationState nesnesi oluşturuluyor, bu nesne kullanıcı kimlik bilgilerini temsil eder.
                var authState = new AuthenticationState(principal);

                // Asenkron bir görev olarak authState'i sakla.
                _authStateTask = Task.FromResult(authState);
            }
        }
    }
}
