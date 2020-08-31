using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailRecipientService : IEmailRecipientService
    {
        private readonly IOptions<EmailSettings> _emailOptions;

        public EmailRecipientService(IOptions<EmailSettings> emailOptions)
        {
            _emailOptions = emailOptions;
        }

        public IEnumerable<string> GetRecipients(ExpenseAuthorizationRequest request)
        {
            // based on the region, we may need to have different To addresses

            yield return _emailOptions.Value.To;
        }
    }
}