﻿using FluentEmail.Core;
using PraksaProjektBackend.Auth;

namespace PraksaProjektBackend.Services

{
    public interface IMailService
    {
        Task<UserManagerResponse> SendEmailAsync(ForgotPassword forgotPassword);

        Task<dynamic> SendQrEmailAsync(string chargeid, string usermail);

        Task<dynamic> SendReservedQrEmailAsync(string eventname, string usermail);

        Task<dynamic> SendNewsletter(IFluentEmail mailer,string subject, string body, string usermail);
    }
}
