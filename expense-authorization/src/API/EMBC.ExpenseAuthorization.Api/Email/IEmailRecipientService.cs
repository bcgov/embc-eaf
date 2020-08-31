using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailRecipientService
    {
        IEnumerable<string> GetRecipients(ExpenseAuthorizationRequest request);
    }
}