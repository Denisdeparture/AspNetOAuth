using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
namespace WebApplication1.Models
{
    public class LoginViewInfoModel
    {
        [Required]
        [Compare("Email", ErrorMessageResourceName = $"Неверная почта")]
        public string Mail { get; set; } = null!;
        [Required]
        [Display(Name = "Пароль")]
        [Compare("Password", ErrorMessageResourceName = $"Неверный пароль")]
        public string Password { get; set; } = null!;
        public string? ReturnUrl { get; set; }
        // 1 google, 2 yandex
        public IList<string> IconUrlLink { get; set; } = new List<string>() {"https://clbrty-img.s3.amazonaws.com/TW:25697989?time=1704554694", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSo2NfBVbUqVSxTEp68r251Xn5uK___XmY5IgHdQgqirQ&s" };
        public IEnumerable<AuthenticationScheme>? genericDatas { get; set; } 

    }
}
