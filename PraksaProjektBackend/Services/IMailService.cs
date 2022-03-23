using PraksaProjektBackend.Auth;

namespace PraksaProjektBackend.Services

{
    public interface IMailService
    {
        Task<UserManagerResponse> SendEmailAsync(ForgotPassword forgotPassword);

        Task<dynamic> SendQrEmailAsync(string chargeid, string usermail);
    }
}
