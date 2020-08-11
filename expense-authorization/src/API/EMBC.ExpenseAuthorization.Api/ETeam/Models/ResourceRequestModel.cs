using System;
using System.ComponentModel.DataAnnotations;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Models
{
    public class ResourceRequestModel
    {
        [StringLength(300)]
        public string ApprovedBy { get; set; }

        public DateTime ApprovedTime { get; set; }

        // StatusResource
        [Required]
        [StringLength(703)]
        public string CurrentStatus { get; set; }

        
        public int EstimatedResourceCost { get; set; }

        public string Mission { get; set; }
        
        [StringLength(48)]
        public string MustComeWithFuel { get; set; }

        [StringLength(48)]
        public string MustComeWithLodging { get; set; }

        [StringLength(48)]
        public string MustComeWithMaint { get; set; }

        [StringLength(48)]
        public string MustComeWithMeals { get; set; }

        [StringLength(48)]
        public string MustComeWithOperator { get; set; }
        
        [StringLength(300)]
        public string MustComeWithOther { get; set; }
        
        [StringLength(48)]
        public string MustComeWithPower { get; set; }
        
        [StringLength(48)]
        public string MustComeWithWater { get; set; }
        
        [StringLength(703)]
        public string Priority { get; set; }

        [StringLength(100)]
        public string QtyUnitOfMeasurement { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 1;
        
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
        
        [Required]
        [StringLength(600)]
        public string ResourceType { get; set; }

        [StringLength(600) /* not sure about this length */]
        public string ResourceTypeTemp { get; set; }
        
        public string SpecialInstructions { get; set; }

        public string SummaryOfActionsTaken { get; set; }

        public DateTime WhenNeeded { get; set; }
    }
}
