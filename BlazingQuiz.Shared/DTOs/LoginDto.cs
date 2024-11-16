using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingQuiz.Shared.DTOs
{
    public class LoginDto
    {
        //UI tarafında bize gönderilecek olan bilgileri tutuyoruz.
        [Required,EmailAddress,DataType(DataType.EmailAddress)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
