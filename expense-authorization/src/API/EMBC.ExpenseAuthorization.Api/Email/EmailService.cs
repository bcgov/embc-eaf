using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRecipientService _recipientService;
        private readonly IOptions<ETeamSettings> _eteamOptions;
        private readonly IEmailSender _sender;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailRecipientService recipientService,
            IOptions<ETeamSettings> eteamOptions,
            IEmailSender sender,
            ILogger<EmailService> logger)
        {
            _recipientService = recipientService ?? throw new ArgumentNullException(nameof(recipientService));
            _eteamOptions = eteamOptions ?? throw new ArgumentNullException(nameof(eteamOptions));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(
            ExpenseAuthorizationRequest request, 
            CreateReportResponse response,
            IList<IFormFile> attachments)
        {
            ETeamSettings eteamSettings = _eteamOptions.Value;

            // create and apply data to the email template
            string content = new EmailTemplate()
                .Apply(request)
                .Apply(response, eteamSettings.Url)
                .Content;

            _logger.LogDebug("Getting the email to recipient list base on request");
            var to = _recipientService.GetToRecipients(request);
            _logger.LogDebug("Email will be sent to {@EmailTo}", to);

            // Request from R. Wainwright, subject line of the email should 
            // be [Region abbreviation + PREOC] – [A new EAF has been submitted to ETEAMS]
            // however, it is difficult to know the region abbreviation

            string subject = "A new EAF has been submitted to ETEAMS";
            if (!string.IsNullOrEmpty(eteamSettings.Environment))
            {
                subject = $"[{eteamSettings.Environment}] {subject}";
            }

            var message = new Message(to, subject, content, attachments);
            message.AddCc(_recipientService.GetCcRecipients(request));
            message.AddBcc(_recipientService.GetBccRecipients(request));

            await _sender.SendEmailAsync(message);
        }
    }
}
