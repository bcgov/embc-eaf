using System;
using System.ComponentModel.DataAnnotations;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public class ETeamSettings
    {
        public const string Section = "ETeam";

        [Required]
        public Uri Url { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string ReportTypeName { get; set; } = "resource_request";

        public string Environment { get; set; }
    }
}
