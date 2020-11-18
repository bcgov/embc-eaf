using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamSoapService
    {
        Task<IList<LookupValue>> GetExpenditureAuthorizationResourceTypesAsync();

        Task<IList<LookupValue>> GetLookupAsync(LookupType lookupType);

        /// <summary>
        /// Creates a resource request report.
        /// </summary>
        /// <param name="expenseAuthorizationRequest">The expense authorization request.</param>
        Task<CreateReportResponse> CreateReportAsync(ExpenseAuthorizationRequest expenseAuthorizationRequest);
    }
}
