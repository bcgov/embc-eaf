using System;
using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class DefaultEmailRecipientService : IEmailRecipientService
    {
        private readonly IOptions<EmailSettings> _emailOptions;

        /// <summary>Initializes a new instance of the <a onclick="return false;" href="EmailRecipientService" originaltag="see">EmailRecipientService</a> class.</summary>
        /// <param name="emailOptions">The email options.</param>
        public DefaultEmailRecipientService(IOptions<EmailSettings> emailOptions)
        {
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
        }

        /// <summary>Gets to recipients.</summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public IList<string> GetToRecipients(ExpenseAuthorizationRequest request) => Split(_ => _.DefaultSendTo);

        /// <summary>Gets the cc recipients.</summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public IList<string> GetCcRecipients(ExpenseAuthorizationRequest request) => Split(_ => _.Cc);

        /// <summary>Gets the BCC recipients.</summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public IList<string> GetBccRecipients(ExpenseAuthorizationRequest request) => Split(_ => _.Bcc);

        private IList<string> Split(Func<EmailSettings, string> fieldSelector)
        {
            var settings = _emailOptions.Value;
            var field = fieldSelector(settings);

            return field.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
