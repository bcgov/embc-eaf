using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public interface IETeamRestService
    {
        [Post("/REST")]
        public Task<string> CreateReportAsync([Query] Dictionary<string, object> data);
    }
}
