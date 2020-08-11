using System.ComponentModel.DataAnnotations;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailSettings
    {
        public const string Section = "Email";

        /// <summary>
        /// Determines if sending email is enabled or not. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        [Required]
        public string From { get; set; }
        [Required]
        public string SmtpServer { get; set; }
        public int Port { get; set; } = 465;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
