using System;

namespace EMBC.ExpenseAuthorization.Api.Models
{
    public class ExpenseAuthorizationRequest
    {
        public string Event { get; set; }
        public DateTime DateTime { get; set; }
        public string EAFNo { get; set; }
        public string EMBCTaskNo { get; set; }
        public string RequestingOrg { get; set; }
        public string ResourceType { get; set; }
        public string AuthName { get; set; }
        public string AuthTelephone { get; set; }
        public string AuthEmail { get; set; }
        public string Description { get; set; }
        public decimal? AmountRequested { get; set; }
        public decimal? ExpenditureNotToExceed { get; set; }
        public EocApprovals EocApprovals { get; set; }
    }
}
