using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailOptions;
        private readonly IEmailRecipientService _recipientService;
        private readonly IOptions<ETeamSettings> _eteamOptions;
        private readonly IEmailSender _sender;

        public EmailService(
            IOptions<EmailSettings> emailOptions,
            IEmailRecipientService recipientService,
            IOptions<ETeamSettings> eteamOptions,
            IEmailSender sender)
        {
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
            _recipientService = recipientService ?? throw new ArgumentNullException(nameof(recipientService));
            _eteamOptions = eteamOptions ?? throw new ArgumentNullException(nameof(eteamOptions));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task SendEmailAsync(
            ExpenseAuthorizationRequest request, 
            CreateReportResponse response,
            IList<IFormFile> attachments)
        {
            EmailSettings emailSettings = _emailOptions.Value;
            ETeamSettings eteamSettings = _eteamOptions.Value;

            // create and apply data to the email template
            string content = new EmailTemplate()
                .Apply(request)
                .Apply(response, eteamSettings.Url)
                .Content;

            var to = _recipientService.GetRecipients(request);
            string subject = "E Team Resource Request";
            if (!string.IsNullOrEmpty(eteamSettings.Environment))
            {
                subject = "[" + eteamSettings.Environment + "] " + subject;
            }

            var message = new Message(to, subject, content, attachments);

            await _sender.SendEmailAsync(message);
        }
    }
}
