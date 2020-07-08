using System.Threading.Tasks;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamSoapService
    {
        [Headers("SOAPAction: urn:extds/XdsReportManager/getPicklistKeywordsRequest")]
        [Post("/services/XdsReportManager")]
        public Task<string> GetPicklistKeywordsAsync([Body] string soapEnvelope);
    }
}
