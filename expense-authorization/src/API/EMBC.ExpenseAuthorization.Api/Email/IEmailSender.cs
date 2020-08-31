using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
