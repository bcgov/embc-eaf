using System.ComponentModel.DataAnnotations;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    /// <summary></summary>
    public class EmailSettings
    {
        /// <summary>The configuration section name</summary>
        public const string Section = "Email";

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

        /// <summary>Gets or sets the SMTP server name.</summary>
        /// <value>The SMTP server.</value>
        ////[Required]
        public string SmtpServer { get; set; }

        /// <summary>Gets or sets the SMTP server port.</summary>
        /// <value>The SMTP server port.</value>
        public int Port { get; set; } = 465;

        /// <summary>Gets or sets the username.</summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }
}
