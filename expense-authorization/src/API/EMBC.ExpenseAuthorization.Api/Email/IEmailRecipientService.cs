using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public interface IEmailRecipientService
    {
        IList<string> GetToRecipients(ExpenseAuthorizationRequest request);
        IList<string> GetCcRecipients(ExpenseAuthorizationRequest request);
        IList<string> GetBccRecipients(ExpenseAuthorizationRequest request);
    }

    public class CommunityEmailRecipient
    {
        public string Community { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
    }
}
