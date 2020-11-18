using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFiles;
using FlatFiles.TypeMapping;
using Microsoft.Extensions.Logging;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class CommunityEmailRecipientsProvider
    {
        private readonly ILogger<CommunityEmailRecipientsProvider> _logger;

        public CommunityEmailRecipientsProvider(ILogger<CommunityEmailRecipientsProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IList<CommunityEmailRecipient> GetCommunityEmailRecipients(string filename)
        {
            try
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
                        var options = new SeparatedValueOptions { IsFirstRecordSchema = true };
                        List<CommunityEmailRecipient> recipients = mapper.Read(reader, options).ToList();
                        _logger.LogDebug("Found {Count} community email recipient from {Filename}", recipients.Count, filename);

                        return recipients;
                    }
                }

                _logger.LogWarning("Mapping {Filename} not found, returning empty result", filename);
                return Array.Empty<CommunityEmailRecipient>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to read file {Filename}, returning empty result", filename);
                return Array.Empty<CommunityEmailRecipient>();
            }
        }
    }
}
