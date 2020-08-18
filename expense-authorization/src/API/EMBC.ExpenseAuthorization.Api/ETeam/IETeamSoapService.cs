using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamSoapService
    {
        Task<IList<LookupValue>> GetExpenditureAuthorizationResourceTypesAsync();

        Task<IList<LookupValue>> GetLookupAsync(LookupType lookupType);

        /// <summary>
        /// Creates a resource request report.
        /// </summary>
        /// <param name="resourceRequest">The resource request.</param>
        Task<CreateReportResponse> CreateReportAsync(ResourceRequestModel resourceRequest);
    }
}
