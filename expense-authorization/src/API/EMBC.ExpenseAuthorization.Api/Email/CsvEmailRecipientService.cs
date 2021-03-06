﻿using System;
using System.Collections.Generic;
using System.Linq;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class CsvEmailRecipientService : IEmailRecipientService
    {
        private readonly IOptions<EmailSettings> _emailOptions;
        private readonly CommunityEmailRecipientsProvider _eEmailRecipientsProvider;
        private readonly ILogger<CsvEmailRecipientService> _logger;

        public CsvEmailRecipientService(IOptions<EmailSettings> emailOptions, CommunityEmailRecipientsProvider eEmailRecipientsProvider, ILogger<CsvEmailRecipientService> logger)
        {
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));
            _eEmailRecipientsProvider = eEmailRecipientsProvider ?? throw new ArgumentNullException(nameof(eEmailRecipientsProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IList<string> GetToRecipients(ExpenseAuthorizationRequest request)
        {
            _logger.LogDebug("Getting the email address for {RequestingOrg}", request.RequestingOrg);
            // do not use the default to list
            var recipients = GetRecipients(request, _ => _.To, _ => Enumerable.Empty<string>());

            if (recipients.Count != 0)
            {
                _logger.LogDebug("Found {@EmailRecipients} ", recipients);
                return recipients;
            }

            _logger.LogInformation("Email recipient not found based on request, using the default email address from configuration");

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
            var defaultService = new DefaultEmailRecipientService(_emailOptions);
            var defaultList = defaultService.GetToRecipients(request);

            if (defaultList.Count == 0)
            {
                _logger.LogError("No default email recipients not found, sending email is probably going to fail");
            }

            return defaultList;
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
            if (request?.RequestingOrg == null)
            {
                _logger.LogInformation("Request or RequestingOrg is null");
                return null;
            }

            IDictionary<string, CommunityEmailRecipient> recipients = GetRecipients();

            var requestingOrg = request.RequestingOrg.Trim();

            if (recipients.TryGetValue(requestingOrg, out var recipient))
            {
                return recipient;
            }

            return null;
        }
        
        private IDictionary<string, CommunityEmailRecipient> GetRecipients()
        {
            var recipients = new Dictionary<string, CommunityEmailRecipient>(StringComparer.OrdinalIgnoreCase);

            // load the file and remove any separators (all underscores)
            var communityRecipients = _eEmailRecipientsProvider
                .GetCommunityEmailRecipients(_emailOptions.Value.RecipientMappingFile)
                .Where(_ => _.Community != null && _.Community.Any(c => c != '_'))
                .ToLookup(_ => _.Community.Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (var community in communityRecipients)
            {
                // if there is more than item, then there is a duplicate in the file
                List<CommunityEmailRecipient> items = community.ToList();

                if (1 < items.Count)
                {
                    _logger.LogInformation("More than one line matched {Community}, only the first matching line will be used", community.Key);
                }

                recipients.Add(community.Key, items[0]);
            }

            return recipients;
        }
    }
}
