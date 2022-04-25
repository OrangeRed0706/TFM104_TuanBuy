using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Business.Services.Utility
{
    public class Mail
    {
        public static void SendMail(string MailTo, string Subject, string Body)
        {
            //參考網址：https://sorceryforce.net/zh-Hant/tips/dot-net-mail-kit-send 
            var smtpHostName = "smtp.gmail.com";
            var smtpPort = 587;
            //var smtpAuthUser = "lynnmaxwell0706@gmail.com";
            //var smtpAuthPassword = "wzczifrrnisecpmi";
            var smtpAuthUser = "tfm104.tuanbuy@gmail.com";
            var smtpAuthPassword = "hfsatcwygrkrwomp";



            var from = "tfm104.tuanbuy@gmail.com";
            //var from = "lynnmaxwell0706@gmail.com";
            var to = MailTo;

            var subject = Subject;
            var body = Body;
            var textFormat = TextFormat.Html;



            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            //內容
            var textPart = new TextPart(textFormat)
            {
                Text = body,
            };
            message.Body = textPart;

            var client = new SmtpClient();


            client.Connect(smtpHostName, smtpPort, SecureSocketOptions.Auto);

            if (string.IsNullOrEmpty(smtpAuthUser) == false)
            {
                client.Authenticate(smtpAuthUser, smtpAuthPassword);
            }


            client.Send(message);
            client.Disconnect(true);
        }

    }
}