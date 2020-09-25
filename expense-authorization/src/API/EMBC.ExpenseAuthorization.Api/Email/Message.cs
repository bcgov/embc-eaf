using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class Message
    {
        public List<MailboxAddress> To { get; private set; } = new List<MailboxAddress>();

        public List<MailboxAddress> Cc { get; private set; } = new List<MailboxAddress>();
        public List<MailboxAddress> Bcc { get; private set; } = new List<MailboxAddress>();

        public string Subject { get; set; }
        public string Content { get; set; }

        public IList<IFormFile> Attachments { get; set; } = Array.Empty<IFormFile>();

        public Message(IEnumerable<string> to, string subject, string content, IList<IFormFile> attachments)
        {
            To.AddRange(to.Select(email => new MailboxAddress(email)));

            Subject = subject;
            Content = content;

            if (attachments != null)
            {
                Attachments = attachments;
            }
        }

        public void AddCc(IEnumerable<string> cc)
        {
            Cc.AddRange(cc.Select(email => new MailboxAddress(email)));
        }

        public void AddBcc(IEnumerable<string> bcc)
        {
            Bcc.AddRange(bcc.Select(email => new MailboxAddress(email)));
        }

    }
}
