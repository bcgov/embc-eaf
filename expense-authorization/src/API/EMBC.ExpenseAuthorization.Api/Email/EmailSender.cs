using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    /// <summary></summary>
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailSettings> _options;
        private readonly ILogger<EmailSender> _logger;

        /// <summary>Initializes a new instance of the <see cref="EmailSender" /> class.</summary>
        /// <param name="options">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">settings
        /// or
        /// logger</exception>
        public EmailSender(IOptions<EmailSettings> options, ILogger<EmailSender> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Sends the email asynchronous.</summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">message</exception>
        public async Task SendEmailAsync(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            EmailSettings settings;

            try
            {
                settings = _options.Value;
            }
            catch (OptionsValidationException exception)
            {
                _logger.LogCritical(exception, "Options (configuration) Validation failure on {OptionsName} for {OptionsType}. Failures: {Failures}",
                    exception.OptionsName,
                    exception.OptionsType,
                    exception.Failures.ToArray());

                throw;
            }

            var emailMessage = await CreateEmailMessageAsync(message, settings.From);

            await SendAsync(emailMessage, settings);
        }

        private async Task<MimeMessage> CreateEmailMessageAsync(Message message, string from)
        {

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(from));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.Content
            };

            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    using var stream = new MemoryStream();
                    await attachment.CopyToAsync(stream);
                    byte[] fileContents = stream.ToArray();

                    bodyBuilder.Attachments.Add(attachment.FileName, fileContents, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage message, EmailSettings settings)
        {
            if (!settings.Enabled)
            {
                _logger.LogInformation("Sending email is disabled");
                return;
            }

            using (var client = new SmtpClient())
            {
                try
                {
                    _logger.LogTrace("Connecting to SMTP server using SSL, {SmtpServer}:{Port}", settings.SmtpServer, settings.Port);
                    await client.ConnectAsync(settings.SmtpServer, settings.Port, settings.Ssl);

                    if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
                    {
                        _logger.LogTrace("Authenticating to SMTP server as {Username}", settings.Username);
                        await client.AuthenticateAsync(settings.Username, settings.Password);
                    }
                    else
                    {
                        _logger.LogTrace("Not authenticating to SMTP server, either the Username or Password is empty");
                    }

                    _logger.LogDebug("Sending email message");
                    await client.SendAsync(message);
                }
                catch (Exception exception)
                {
                    //log an error message or throw an exception or both.
                    _logger.LogError(exception, "Failed to send email message");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
