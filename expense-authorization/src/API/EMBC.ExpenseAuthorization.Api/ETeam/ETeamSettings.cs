using System;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public class ETeamSettings
    {
        public const string Section = "ETeams";

        public Uri Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string ReportTypeName { get; set; }
    }
}
