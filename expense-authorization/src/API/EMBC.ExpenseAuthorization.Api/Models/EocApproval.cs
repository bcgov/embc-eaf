using System;

namespace EMBC.ExpenseAuthorization.Api.Models
{
    public class EocApproval
    {
        public string ApprovedBy { get; set; }
        public string Position { get; set; }
        public DateTime ApprovalDateTime { get; set; }
    }
}