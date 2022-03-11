using PraksaProjektBackend.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using PraksaProjektBackend.Auth;


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


    }
}
