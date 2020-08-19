using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Microsoft.AspNetCore.Http;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(ResourceRequestModel resourceRequest, CreateReportResponse report,  IList<IFormFile> attachments);
    }
}
