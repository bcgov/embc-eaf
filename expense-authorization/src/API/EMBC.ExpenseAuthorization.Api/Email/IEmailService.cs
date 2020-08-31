using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.AspNetCore.Http;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(ExpenseAuthorizationRequest request, CreateReportResponse response, IList<IFormFile> attachments);
    }
}
