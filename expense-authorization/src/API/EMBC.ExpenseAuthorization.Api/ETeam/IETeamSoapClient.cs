using System.Threading.Tasks;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    /// <summary>
    /// Refit generated service.
    /// </summary>
    public interface IETeamSoapClient
    {
        [Headers("SOAPAction: urn:extds/XdsReportManager/getPicklistColors")]
        [Post("/services/XdsReportManager")]
        public Task<string> GetPicklistColorsAsync([Body] string soapEnvelope);

        [Headers("SOAPAction: urn:extds/XdsReportManager/getPicklistKeywordsRequest")]
        [Post("/services/XdsReportManager")]
        public Task<string> GetPicklistKeywordsAsync([Body] string soapEnvelope);
        
        [Headers("SOAPAction: urn:extds/XdsReportManager/createRequest")]
        [Post("/services/XdsReportManager")]
        public Task<string> CreateReportAsync([Body] string soapEnvelope);

        [Headers("SOAPAction: urn:extds/XdsReportManager/loginRequest")]
        [Post("/services/XdsReportManager")]
        public Task<string> LoginAsync([Body] string soapEnvelope);

    }
}
