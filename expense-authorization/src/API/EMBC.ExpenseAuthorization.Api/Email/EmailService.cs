using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
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
            _emailOptions = emailOptions;
            _eteamOptions = eteamOptions;
            _sender = sender;
        }

        public async Task SendEmailAsync(CreateReportResponse report, IList<IFormFile> attachments)
        {
            EmailSettings emailSettings = _emailOptions.Value;

            var message = new Message
            {
                To = new List<MailboxAddress> { new MailboxAddress(emailSettings.From) },
                Attachments = attachments,
                Content = CreateEmailContent(report, attachments),
                Subject = "E-Team Resource Request",
            };

            await _sender.SendEmailAsync(message);

        }

        private string CreateEmailContent(CreateReportResponse report, IList<IFormFile> attachments)
        {
            ETeamSettings eteamSettings = _eteamOptions.Value;

            // https://host/instance/report/resource.do?target=read&id=id&reportType=resource_request
            string reportUrl = eteamSettings.Url + "report/resource.do?target=read&reportType=resource_request&id" + report.Fields["id"];

            string template = EmbeddedResource.Get<EmailService>("email_template.html");

            template = template.Replace("{{ReportUrl}}", reportUrl);
            template = template.Replace("{{DateOfRequest}}", string.Empty);
            template = template.Replace("{{EAFNumber}}", string.Empty);
            template = template.Replace("{{DescriptionOfExpenditure}}", string.Empty);
            template = template.Replace("{{Event}}", string.Empty);
            template = template.Replace("{{Event}}", string.Empty);
            template = template.Replace("{{ExpenditureNotToExceed}}", string.Empty);

            return template;
        }
    }
}
