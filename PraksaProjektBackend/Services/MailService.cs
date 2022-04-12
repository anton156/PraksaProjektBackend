using PraksaProjektBackend.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using PraksaProjektBackend.Auth;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;

namespace PraksaProjektBackend.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public MailService(IOptions<MailSettings> options, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _mailSettings = options.Value;
            _userManager = userManager;
            _configuration = configuration;

        }
        public async Task<UserManagerResponse> SendEmailAsync(ForgotPassword forgotPassword)
        {
            var user = await _userManager.FindByEmailAsync(forgotPassword.ToEmail);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(forgotPassword.ToEmail));
            email.Subject = "Password recovery";

            var builder = new BodyBuilder();
            builder.HtmlBody = "<h3>Reset token: </h3>" + $"{token}";

            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset token has been sent to the email successfully!"
            };
        }

        public async Task<dynamic> SendQrEmailAsync(string chargeid, string usermail)
        {

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(usermail));
            email.Subject = "Ticket QR Code";

            var builder = new BodyBuilder();
            var diskpath = "wwwroot/QRcode/QR_" + chargeid + ".jpg";
            var imgpath = "https://localhost:7100/QR_" + chargeid + ".jpg";
            var bodyhtml = "<html><body> <p> QR code as below</p> <p> <img src='" + imgpath + "' alt='QR Code'/></p> </body></html>";
            builder.HtmlBody = bodyhtml;

            builder.Attachments.Add(diskpath);

            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return "Success";
        }

        public async Task<dynamic> SendReservedQrEmailAsync(string eventname, string usermail)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(usermail));
            email.Subject = "Reserved Ticket QR Code";
            var qr = await QrCodeMaker.ReservedTicket(eventname);
            var builder = new BodyBuilder();
            var diskpath = "wwwroot/ReservedQRcode/QR_" + eventname + ".jpg";

            builder.Attachments.Add(diskpath);

            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return "Success";
        }

        public async Task<dynamic> SendNewsletter(IFluentEmail mailer,string subject, string body, string usermail)
        {
            //var email = new MimeMessage();
            //email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            //email.To.Add(MailboxAddress.Parse(usermail));
            //email.Subject = subject;

            //var builder = new BodyBuilder();

            //builder.HtmlBody = "<html><body> <p>" + body + "</p> </body></html>"; 

            //email.Body = builder.ToMessageBody();
            //using var smtp = new SmtpClient();
            //smtp.CheckCertificateRevocation = false;
            //smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            //await smtp.SendAsync(email);
            //smtp.Disconnect(true);

            var email = mailer
                        .To(usermail)
                        .Subject(subject)
                        .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/TemplateEmail/TemplateEmail.cshtml",
                    new
                    {
                        Subject = subject,
                        Body = body,
                    });
            //.Body(body);

            await email.SendAsync();

            return "Success";
        }
    }
}
