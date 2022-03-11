using PraksaProjektBackend.Auth;

namespace PraksaProjektBackend.Services

{
    public interface IMailService
    {
        Task<UserManagerResponse> SendEmailAsync(ForgotPassword forgotPassword);
    }
}
