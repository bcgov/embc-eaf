using System;
using System.Collections.Generic;
using System.Text;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public class ExpenseAuthorizationRequestMapper : IExpenseAuthorizationRequestMapper
    {
        public IDictionary<string, string> Map(
            ExpenseAuthorizationRequest source, 
            string priority,
            string resourceCategory,
            string currentStatus
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (priority == null)
            {
                throw new ArgumentNullException(nameof(priority));
            }

            if (resourceCategory == null)
            {
                throw new ArgumentNullException(nameof(resourceCategory));
            }

            if (currentStatus == null)
            {
                throw new ArgumentNullException(nameof(currentStatus));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var items = new Dictionary<string, string>();

            items.Add("reportType", "resource_request");
            items.Add("quantity", "1");
            items.Add("priority", priority);
            items.Add("currentStatus", currentStatus);
            items.Add("resourceCategory", resourceCategory);            // Expenditure Authorization
            items.Add("approvedTime", source.DateTime.ToString("o"));
            items.Add("requestorContactInfo", FormatContactInfo(source));
            items.Add("resourceType", source.ResourceType);
            items.Add("requestNumber", source.EAFNo);
            items.Add("reqTrackNoState", source.EMBCTaskNo);
            items.Add("requestionOrg", source.RequestingOrg);
            items.Add("mission", FormatMission(source));
            items.Add("estimatedResourceCost", FormatEstimatedResourceCost(source));

            return items;
        }

        private string FormatContactInfo(ExpenseAuthorizationRequest source)
        {
            StringBuilder buffer = new StringBuilder();
            if (!string.IsNullOrEmpty(source.AuthName))
            {
                buffer.Append("Name: ");
                buffer.Append(source.AuthName);
            }

            if (!string.IsNullOrEmpty(source.AuthTelephone))
            {
                if (buffer.Length != 0)
                {
                    buffer.Append('\n');
                }
                buffer.Append("Tel: ");
                buffer.Append(source.AuthTelephone);
            }

            if (!string.IsNullOrEmpty(source.AuthEmail))
            {
                if (buffer.Length != 0)
                {
                    buffer.Append('\n');
                }
                buffer.Append("Email: ");
                buffer.Append(source.AuthEmail);
            }

            return buffer.ToString();
        }

        private string FormatMission(ExpenseAuthorizationRequest source)
        {
            int amountRequested = (int)Math.Ceiling(source.AmountRequested);
            return source.Description + "\nAmount Requested: " + amountRequested;
        }

        private string FormatEstimatedResourceCost(ExpenseAuthorizationRequest source)
        {
            int expenditureNotToExceed = (int)Math.Ceiling(source.ExpenditureNotToExceed);
            return expenditureNotToExceed.ToString();
        }
    }
}
