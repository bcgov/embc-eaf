using System.Xml.Linq;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    public class LoginResponse : SoapResponse
    {
        protected override void ReadElementsFromXml(XElement element)
        {
            if (element.Name.LocalName != "loginResponse")
            {
                // error
            }
        }
    }
}
