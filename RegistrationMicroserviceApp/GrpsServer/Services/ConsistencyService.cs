using Grpc.Core;
using GrpsServer;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Diagnostics;
using WebClient.AppContext;
using WebClient.Models.DataModel;
namespace GrpsServer.Services
{
    public class ConsistencyService : GrpsServer.Consistency.ConsistencyBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<DataUserModel> _userManager;
        public ConsistencyService(UserManager<DataUserModel> usermanager,  IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = usermanager;
        }
        public override Task<Request> DataMethod(Request request, ServerCallContext context)
        {
            // Тут реализована и запись пользователя и отправка ему сообщения оь успешной регистацтии
            
      //      string answerServer = "Complete. User was add";

            DataUserModel newUser = new DataUserModel()
            {
                Email = request.Mail,
                PasswordHash = request.Password,
            };
            bool redflag = true;
            if(_userManager.FindByEmailAsync(request.Mail) == null) redflag = false;
           
            if(redflag)
            {
                _userManager.CreateAsync(newUser);
                try
                {
                    SendMail(newUser);
                }catch { }
                // Потом возвращаешь данные и регистрируеш их на клиенте 
                return Task.FromResult(request);
            }
            return null;
        }
        public override Task<UserCkeck> DataCheck(Request request, ServerCallContext context)
        {
            bool userExesists = false;
            //DataUserModel newUser = new DataUserModel()
            //{
            //    Email = request.Mail,
            //    PasswordHash = request.Password,
            //};
            Debug.WriteLine("Begin");
            if(_userManager.FindByEmailAsync(request.Mail) != null) userExesists = true;
            return Task.FromResult(new UserCkeck
            {
                Check = userExesists

            });
        }
        private void SendMail(DataUserModel user)
        {
            using(var mailbroker = new SmtpClient())
            {
                mailbroker.Connect("smtp.yandex.ru",465,true);
                string yaUser = _configuration.GetSection("YandexUser").Get<string>()!;
                string yaPass = _configuration.GetSection("YandexPass").Get<string>()!;
                mailbroker.Authenticate(yaUser,yaPass);
                var msg = new MimeMessage()
                {
                    Subject = "Congratulations on your successful registration on our service!",
                    Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = $"Hello!{user.Email}.Your data in my base"},
                };

                msg.To.Add(new MailboxAddress("", user.Email));
                msg.From.Add(new MailboxAddress("Администрация сайта", "ilimbaevAshitaDenisLD@yandex.ru"));
                mailbroker.Send(msg);
            }
        }
    }
}
