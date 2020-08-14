using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamSoapService
    {
        /// <summary>
        /// Gets the pick list lookup.
        /// </summary>
        /// <param name="lookupType"></param>
        /// <returns></returns>
        /// <exception cref="ApiException">
        /// Error calling SOAP service.  See <see cref="ApiException.Content"/> for more information.
        /// </exception>
        Task<IList<LookupValue>> GetPicklistKeywordsAsync(LookupType lookupType);

        Task<IList<LookupValue>> GetPicklistColorsAsync(LookupType lookupType);

        Task<CreateReportResponse> CreateReportAsync(ResourceRequestModel resourceRequest);
    }
}
