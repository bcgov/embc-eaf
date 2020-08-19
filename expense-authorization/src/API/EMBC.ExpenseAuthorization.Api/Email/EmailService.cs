using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailOptions;
        private readonly IOptions<ETeamSettings> _eteamOptions;
        private readonly IEmailSender _sender;

        public EmailService(IOptions<EmailSettings> emailOptions, IOptions<ETeamSettings> eteamOptions, IEmailSender sender)
        {
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
            _eteamOptions = eteamOptions ?? throw new ArgumentNullException(nameof(eteamOptions));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task SendEmailAsync(
            ResourceRequestModel resourceRequest, 
            CreateReportResponse report,
            IList<IFormFile> attachments)
        {
            EmailSettings emailSettings = _emailOptions.Value;
            ETeamSettings eteamSettings = _eteamOptions.Value;

            // create and apply data to the email template
            string content = new EmailTemplate()
                .Apply(resourceRequest)
                .Apply(report, eteamSettings.Url)
                .Content;

            // based on the region, we may need to have different To addresses

            var message = new Message
            {
                To = new List<MailboxAddress> { new MailboxAddress(emailSettings.From) },
                Attachments = attachments,
                Content = content,
                Subject = "E-Team Resource Request",
            };

            await _sender.SendEmailAsync(message);
        }
    }
}
