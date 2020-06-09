using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.XPath;
using Newtonsoft.Json;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamRestService
    {
        [Post("/REST")]
        public Task CreateReportAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);
    }
}
