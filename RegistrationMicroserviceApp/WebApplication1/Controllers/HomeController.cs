using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using Grpc.Net.Client;
using WebClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using WebClient.Models.DataModel;
using WebClient.Models;
using Microsoft.AspNetCore.Mvc.Routing;
namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly SignInManager<DataUserModel> _signInManager;
        public HomeController(SignInManager<DataUserModel> signInManager)
        {
            _signInManager = signInManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewData["UserData"] = User.Identity.Name ?? "";
            return View();
        }
        [HttpGet]
        [ActionName("Authentication")]
        public async Task< IActionResult>LoginGet(string returnUrl)
        {
            return View(new LoginViewInfoModel() { genericDatas = await _signInManager.GetExternalAuthenticationSchemesAsync() , ReturnUrl = returnUrl});
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("Authentication")]
        public async Task<IActionResult>LoginPost(LoginViewInfoModel user)
        {
            if (!ModelState.IsValid) ModelState.AddModelError("", "Password is incorrecd");
            bool IsLongSave = false;
            using (var channel = GrpcChannel.ForAddress("http://localhost:5275"))
            {
                var server = new Consistency.ConsistencyClient(channel);
                var res = server.DataCheck(new Request() { 
                    Mail = user.Mail, 
                    Password = user.Password, 
                });
                Console.WriteLine(res.Check);
                if (!res.Check)
                {
                    return View(user);
                }
            }
            await _signInManager.SignInAsync(new DataUserModel() { Email = user.Mail, PasswordHash = user.Password }, IsLongSave);
            return RedirectToAction(user.ReturnUrl);
        }
        // returnurl - это путь по которому проходищь после регистации, посмотри во вьюхти авторизации
        public IActionResult ProviderAuthentication(string provider, string returnurl)
        {
            // здесь что-то в духе рефлексии 3 аргумент это аргумент передаваемый в функцию
            var actionUrlFormRedirect = Url.Action(nameof(ProviderAuthenticationCallback), "Home", new {returnurl});
            var prop = _signInManager.ConfigureExternalAuthenticationProperties(provider, actionUrlFormRedirect);

            return Challenge(prop,provider);
            // 
          //  return Challenge(new AuthenticationProperties { RedirectUri = "/Home/ProviderAuthenticationCallback" }, provider);
        }
        public async Task<IActionResult> ProviderAuthenticationCallback(string returnUrl)
        {
            ExternalLoginInfo obj = await _signInManager.GetExternalLoginInfoAsync(returnUrl);
            
            //var obj = new ExternalLoginInfo(User, )
            if(obj == null)
            {
                Debug.WriteLine("Ошибка в регистрации через стороний сервис");
               // Console.WriteLine("Ошибка в регистрации через стороний сервис");
                return RedirectToAction("Authentication");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(obj.LoginProvider, obj.ProviderKey, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
          //  var actionUrlFormRedirect = Url.Action(nameof(RegistrationPost), "" );
            return RedirectToAction("Authorization", new RegisterViewInfoModel() { Mail = obj.Principal.FindFirstValue(ClaimTypes.Email) ?? obj.Principal.FindFirstValue(ClaimTypes.Name), Password = obj.ProviderKey });
        }

        [HttpGet]
        [ActionName("Authorization")]
        public async Task<IActionResult> RegistrationGet([FromQuery] RegisterViewInfoModel user)
        {
            if(user != null)
            {
                bool IsLongSave = false;
                using (var channel = GrpcChannel.ForAddress("http://localhost:5275"))
                {
                    var server = new Consistency.ConsistencyClient(channel);
                    var res = server.DataMethod(new Request()
                    {
                        Mail = user.Mail,
                        Password = user.Password,
                    });
                    if (res != null)
                    {
                        await _signInManager.SignInAsync(new DataUserModel() { Email = user.Mail, UserName = (string.IsNullOrEmpty(user.Mail.Substring(0, user.Mail.IndexOf("@")))) ? user.Mail : user.Mail.Substring(0, user.Mail.IndexOf("@")), PasswordHash = user.Password }, IsLongSave);
                    }
                }

                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        [ActionName("Authorization")]
        public async Task<IActionResult> RegistrationPost([FromForm]RegisterViewInfoModel user)
        {
            bool IsLongSave = false;
            Console.WriteLine(user.Password);
            Console.WriteLine(user.Mail);
            using (var channel = GrpcChannel.ForAddress("http://localhost:5275"))
            {
                var server = new Consistency.ConsistencyClient(channel);
                var res = server.DataMethod(new Request() {
                    Mail = user.Mail,
                    Password = user.Password,
                });
                if(res != null)
                {
                  await  _signInManager.SignInAsync(new DataUserModel() { Email = user.Mail, UserName = user.Name + user.LastName, PasswordHash = user.Password }, IsLongSave);
                }
            }
            
            return RedirectToAction("Index");
        }
        public IActionResult LoginOut()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Authentication");
        }


        //private async Task Authenticate(LoginViewInfoModel user)
        //{
        //var claims = new List<Claim>
        //    {
        //        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)

        //    };
        //ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
