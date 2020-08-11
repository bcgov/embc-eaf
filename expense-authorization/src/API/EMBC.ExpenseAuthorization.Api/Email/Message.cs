using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }

        public IList<IFormFile> Attachments { get; set; } = Array.Empty<IFormFile>();

        public Message()
        {
        }

        public Message(string to, string subject, string content, IList<IFormFile> attachments)
        {
            To = new List<MailboxAddress> {new MailboxAddress(to)};
            Subject = subject;
            Content = content;

            if (attachments != null)
            {
                Attachments = attachments;
            }
        }

        public Message(IEnumerable<string> to, string subject, string content, IList<IFormFile> attachments)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
            Content = content;

            if (attachments != null)
            {
                Attachments = attachments;
            }
        }
    }
}
