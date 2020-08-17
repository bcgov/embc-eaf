namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public class GetPicklistKeywordsRequest : GetPicklistRequest
    {
        public GetPicklistKeywordsRequest(LookupType lookupType) : base("getPicklistKeywords")
        {
            Key = lookupType.ToString();
        }
    }
}
