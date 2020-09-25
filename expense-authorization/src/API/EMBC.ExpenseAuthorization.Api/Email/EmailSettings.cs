using System;
using System.Collections.Generic;
using System.Linq;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    /// <summary></summary>
    public class EmailSettings
    {
        /// <summary>The configuration section name</summary>
        public const string Section = "Email";

        public string RecipientMappingFile { get; set; }

        /// <summary>
        /// Determines if sending email is enabled or not. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets email address to send from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        ////[Required]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the default to send list of no other valid email address are configured.
        /// </summary>
        public string DefaultSendTo { get; set; }
        
        /// <summary>
        /// Gets or sets addresses to cc to.
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// Gets or sets addresses to bcc to.
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>Gets or sets the SMTP server name.</summary>
        /// <value>The SMTP server.</value>
        ////[Required]
        public string SmtpServer { get; set; }

        /// <summary>Gets or sets the SMTP server port.</summary>
        /// <value>The SMTP server port.</value>
        public int? Port { get; set; }

        public bool Ssl { get; set; }
        
        /// <summary>Gets or sets the username.</summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        public IEnumerable<string> GetCc() => GetList(Cc);

        public IEnumerable<string> GetBcc() => GetList(Bcc);

        private IEnumerable<string> GetList(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return Enumerable.Empty<string>();
            }

            var items = source.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return items;
        }
    }
}
