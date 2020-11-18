using System.Threading.Tasks;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
