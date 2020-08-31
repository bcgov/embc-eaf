using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    /// <summary>
    /// Represents a class that will map the <see cref="ExpenseAuthorizationRequest"/> into a dictionary
    /// that can be used to send to Eteam.
    /// </summary>
    public interface IExpenseAuthorizationRequestMapper
    {
        IDictionary<string, string> Map(ExpenseAuthorizationRequest source, string priority, string resourceCategory, string currentStatus);
    }
}
