using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EMBC.ExpenseAuthorization.Api.Models;
using FlatFiles;
using FlatFiles.TypeMapping;
using Microsoft.Extensions.Options;
using Serilog;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class CsvEmailRecipientService : IEmailRecipientService
    {
        private readonly IOptions<EmailSettings> _emailOptions;
        private readonly ILogger _logger;

        public CsvEmailRecipientService(IOptions<EmailSettings> emailOptions, ILogger logger)
        {
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IList<string> GetToRecipients(ExpenseAuthorizationRequest request)
        {
            // do not use the default to list
            var recipients = GetRecipients(request, _ => _.To, _ => Enumerable.Empty<string>());

            if (recipients.Count != 0)
            {
                return recipients;
            }

            // if the config file does not have configured recipient, fall back to the configuration
            return GetDefaultToEmailRecipients(request);
        }

        public IList<string> GetCcRecipients(ExpenseAuthorizationRequest request)
        {
            var recipients = GetRecipients(request, _ => _.Cc, _ => _.GetCc());
            return recipients;
        }

        public IList<string> GetBccRecipients(ExpenseAuthorizationRequest request)
        {
            var recipients = GetRecipients(request, _ => _.Bcc, _ => _.GetBcc());
            return recipients;
        }

        private IList<string> GetDefaultToEmailRecipients(ExpenseAuthorizationRequest request)
        {
            // if the config file does not have configured recipient, fall back to the configuration
            DefaultEmailRecipientService defaultEmailRecipientService = new DefaultEmailRecipientService(_emailOptions);
            return defaultEmailRecipientService.GetToRecipients(request);
        }

        private IList<string> GetRecipients(ExpenseAuthorizationRequest request, Func<CommunityEmailRecipient, string> fieldSelector, Func<EmailSettings, IEnumerable<string>> defaultRecipients)
        {
            List<string> recipients = new List<string>();

            var recipient = GetCommunityEmailRecipient(request);

            if (recipient != null)
            {
                var field = fieldSelector(recipient);

                if (!string.IsNullOrEmpty(field))
                {
                    recipients.AddRange(field.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            var settings = _emailOptions.Value;
            recipients.AddRange(defaultRecipients(settings));

            return recipients;
        }


        private CommunityEmailRecipient GetCommunityEmailRecipient(ExpenseAuthorizationRequest request)
        {
            IDictionary<string, CommunityEmailRecipient> recipients = GetRecipients();

            if (recipients.TryGetValue(request.RequestingOrg, out var recipient))
            {
                return recipient;
            }

            return null;
        }
        
        private IDictionary<string, CommunityEmailRecipient> GetRecipients()
        {
            var recipients = new Dictionary<string, CommunityEmailRecipient>(StringComparer.OrdinalIgnoreCase);

            var loader = new CommunityEmailRecipientsProvider(_logger);
            var settings = _emailOptions.Value;

            // load the file and remove any separators
            var communityRecipients = loader
                .GetCommunityEmailRecipients(settings.RecipientMappingFile)
                .Where(_ => _.Community.Any(c => c != '_'))
                .ToLookup(_ => _.Community, StringComparer.OrdinalIgnoreCase);

            foreach (var community in communityRecipients)
            {
                // if there is more than item, then there is a duplicate in the file
                List<CommunityEmailRecipient> items = community.ToList();

                if (1 < items.Count)
                {
                    _logger.Information("More than one line matched {Community}, only the first matching line will be used", community.Key);
                }

                recipients.Add(community.Key, items[0]);
            }

            return recipients;
        }
        

    }

    public class CommunityEmailRecipientsProvider
    {
        private readonly ILogger _logger;

        public CommunityEmailRecipientsProvider(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IList<CommunityEmailRecipient> GetCommunityEmailRecipients(string filename)
        {
            var mapper = SeparatedValueTypeMapper.Define<CommunityEmailRecipient>();
            mapper.Property(c => c.Community).ColumnName("community");
            mapper.Property(c => c.To).ColumnName("to");
            mapper.Property(c => c.Cc).ColumnName("cc");
            mapper.Property(c => c.Bcc).ColumnName("bcc");

            if (File.Exists(filename))
            {
                using (var reader = new StreamReader(File.OpenRead(filename)))
                {
                    var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                    List<CommunityEmailRecipient> recipients = mapper.Read(reader, options).ToList();
                    return recipients;
                }
            }

            _logger.Warning("Mapping {Filename} not found", filename);
            return Array.Empty<CommunityEmailRecipient>();
        }
    }
}
