using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazingQuiz.Shared
{
    public record LoggedInUser(int Id, string Name, string Role, string Token)
    {
        public string ToJson() => JsonSerializer.Serialize(this);

        public Claim[] ToClaims() =>
          [

                new Claim(ClaimTypes.NameIdentifier, Id.ToString()), // Kullanıcı ID'si
                new Claim(ClaimTypes.Name, Name), // Kullanıcı adı
                new Claim(ClaimTypes.Role, Role), // Kullanıcı rolü
                new Claim(nameof(Token), Token) // Kullanıcı rolü
                
            ];

        public static LoggedInUser? LoadFrom(string json) =>
            !string.IsNullOrWhiteSpace(json) 
            ? JsonSerializer.Deserialize<LoggedInUser>(json)
            : null;
    }

 

}
