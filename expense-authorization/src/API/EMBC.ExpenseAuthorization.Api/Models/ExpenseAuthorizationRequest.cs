using System;

namespace EMBC.ExpenseAuthorization.Api.Models
{
    public class ExpenseAuthorizationResponse
    {
        public string Id { get; set; }
    }

    public class ExpenseAuthorizationRequest
    {
        private decimal _amountRequested;
        private decimal _expenditureNotToExceed;
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

        public decimal AmountRequested
        {
            get
            {
                return _amountRequested;
            }
            set
            {
                // always round up
                _amountRequested = Math.Ceiling(value);
            }
        }

        public decimal ExpenditureNotToExceed
        {
            get
            {
                return _expenditureNotToExceed;
            }
            set
            {
                // always round up
                _expenditureNotToExceed = Math.Ceiling(value);
            }
        }

        public EocApprovals EocApprovals { get; set; }
    }
}
