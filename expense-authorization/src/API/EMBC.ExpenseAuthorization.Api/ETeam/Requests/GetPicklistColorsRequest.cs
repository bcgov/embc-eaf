namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public class GetPicklistColorsRequest : GetPicklistRequest
    {
        public GetPicklistColorsRequest(LookupType lookupType) : base("getPicklistColors")
        {
            Key = lookupType.ToString();
        }
    }
}