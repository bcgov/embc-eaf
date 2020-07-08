using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public static class ETeamRestServiceExtensions
    {
        public static Task CreateReportAsync(this IETeamRestService service, string username, string password, string reportTypeName, ResourceRequestModel resourceRequest)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Parameter cannot be null or empty", nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Parameter cannot be null or empty", nameof(password));
            if (string.IsNullOrEmpty(reportTypeName)) throw new ArgumentException("Parameter cannot be null or empty", nameof(reportTypeName));

            Dictionary<string, object> request = new Dictionary<string, object>
            {
                {"_u", username},
                {"_p", password},
                {"_m", "PUT"},
                {"_r", "report"},
                {"_t", reportTypeName}
            };

            AddIfNotNullOrEmpty(request, "approvedBy", resourceRequest.ApprovedBy);
            AddIfNotDefault(request, "approvedTime", resourceRequest.ApprovedTime);
            AddIfNotNullOrEmpty(request, "currentStatus", resourceRequest.CurrentStatus);
            Add(request, "estimatedResourceCost", resourceRequest.EstimatedResourceCost);
            AddIfNotNullOrEmpty(request, "mission", resourceRequest.Mission);
            AddIfNotNullOrEmpty(request, "mustComeWithFuel", resourceRequest.MustComeWithFuel);
            AddIfNotNullOrEmpty(request, "mustComeWithLodging", resourceRequest.MustComeWithLodging);
            AddIfNotNullOrEmpty(request, "mustComeWithMaint", resourceRequest.MustComeWithMaint);
            AddIfNotNullOrEmpty(request, "mustComeWithMeals", resourceRequest.MustComeWithMeals);
            AddIfNotNullOrEmpty(request, "mustComeWithOperator", resourceRequest.MustComeWithOperator);
            AddIfNotNullOrEmpty(request, "mustComeWithOther", resourceRequest.MustComeWithOther);
            AddIfNotNullOrEmpty(request, "mustComeWithPower", resourceRequest.MustComeWithPower);
            AddIfNotNullOrEmpty(request, "mustComeWithWater", resourceRequest.MustComeWithWater);
            AddIfNotNullOrEmpty(request, "priority", resourceRequest.Priority);
            AddIfNotNullOrEmpty(request, "qtyUnitOfMeasurement", resourceRequest.QtyUnitOfMeasurement);
            Add(request, "quantity", resourceRequest.Quantity);
            AddIfNotNullOrEmpty(request, "reqTrackNoEmac", resourceRequest.ReqTrackNoEmac);
            AddIfNotNullOrEmpty(request, "reqTrackNoFema", resourceRequest.ReqTrackNoFema);
            AddIfNotNullOrEmpty(request, "reqTrackNoState", resourceRequest.ReqTrackNoState);
            AddIfNotNullOrEmpty(request, "requestNumber", resourceRequest.RequestNumber);
            AddIfNotNullOrEmpty(request, "requestionOrg", resourceRequest.RequestionOrg);
            AddIfNotNullOrEmpty(request, "requestorContactInfo", resourceRequest.RequestorContactInfo);
            AddIfNotNullOrEmpty(request, "resourceCategory", resourceRequest.ResourceCategory);
            AddIfNotNullOrEmpty(request, "resourceType", resourceRequest.ResourceType);
            AddIfNotNullOrEmpty(request, "resourceTypeTemp", resourceRequest.ResourceTypeTemp);
            AddIfNotNullOrEmpty(request, "specialInstructions", resourceRequest.SpecialInstructions);
            AddIfNotNullOrEmpty(request, "summaryOfActionsTaken", resourceRequest.SummaryOfActionsTaken);
            AddIfNotDefault(request, "whenNeeded", resourceRequest.WhenNeeded);
            
            return service.CreateReportAsync(request);
        }

        private static void AddIfNotNullOrEmpty(Dictionary<string, object> request, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                request.Add(name, value);
            }
        }

        private static void AddIfNotDefault(Dictionary<string, object> request, string name, DateTime value)
        {
            if (value != default)
            {
                request.Add(name, value);
            }
        }

        private static void Add(Dictionary<string, object> request, string name, int value)
        {
            request.Add(name, value);
        }
    }
}
