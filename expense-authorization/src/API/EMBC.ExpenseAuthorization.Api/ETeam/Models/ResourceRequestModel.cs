using System;
using System.ComponentModel.DataAnnotations;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Models
{
    public class ResourceRequestModel
    {
        [StringLength(300)]
        public string ApprovedBy { get; set; }

        public DateTime ApprovedTime { get; set; }

        [StringLength(703)]
        public string CurrentStatus { get; set; }

        public int EstimatedResourceCost { get; set; }

        public string Mission { get; set; }

        [StringLength(703)]
        public string Priority { get; set; }
        
        [StringLength(100)]
        public string ReqTrackNoEmac { get; set; }
        
        [StringLength(100)]
        public string ReqTrackNoFema { get; set; }
        
        [StringLength(100)]
        public string ReqTrackNoState { get; set; }
        
        [StringLength(300)]
        public string RequestNumber { get; set; }

        [StringLength(300)]
        public string RequestionOrg { get; set; }
        
        [StringLength(1000)]
        public string RequestorContactInfo { get; set; }
        
        [StringLength(300)]
        public string ResourceCategory { get; set; }
        
        [StringLength(600)]
        public string ResourceType { get; set; }

        public DateTime WhenNeeded { get; set; }

        #region Fields Not Used In API or are defaulted
        // The items below that are commented out are defined in ETeam but are not used 
        // by this application. They have been left here in case they need to be added in the future.
        //[StringLength(600) /* not sure about this length */]
        //public string ResourceTypeTemp { get; set; }
        ////public string SummaryOfActionsTaken { get; set; }
        ////public string SpecialInstructions { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithFuel { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithLodging { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithMaint { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithMeals { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithOperator { get; set; }
        ////[StringLength(300)]
        ////public string MustComeWithOther { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithPower { get; set; }
        ////[StringLength(48)]
        ////public string MustComeWithWater { get; set; }
        ////[StringLength(100)]
        ////public string QtyUnitOfMeasurement { get; set; }
        ////[Range(0, int.MaxValue)]
        ////public int Quantity { get; set; } = 1;
        #endregion

        #region Extra Fields Not Sent to ETeam
        public string Event { get; set; }
        public decimal ExpenditureNotToExceed { get; set; }

        public EocApprovals EocApprovals { get; set; }
        #endregion
    }

    public class EocApprovals
    {
        public EocApproval Processing { get; set; }
        public EocApproval ExpenditureRequest { get; set; }
    }

    public class EocApproval
    {
        public string ApprovedBy { get; set; }
        public string Position { get; set; }
        public DateTime ApprovalDateTime { get; set; }
    }
}
