using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public static class ETeamSoapServiceExtensions
    {
        public static async Task<List<LookupValue>> GetPicklistKeywords(this IETeamSoapService service, LookupType lookupType)
        {
            // create the correct SOAP Envelope for the request
            string soapEnvelope = CreateGetPicklistKeywordsRequest(lookupType.ToString());

            // make SOAP request and get the response SOAP Envelope
            soapEnvelope = await service.GetPicklistKeywordsAsync(soapEnvelope);

            return GetPicklistValues(soapEnvelope);
        }

        private static string CreateGetPicklistKeywordsRequest(string key)
        {
            string format = @"
<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:extds"">
   <soapenv:Header/>
   <soapenv:Body>
      <urn:getPicklistKeywords soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
         <key xsi:type=""xsd:string"">{0}</key> 
      </urn:getPicklistKeywords>
   </soapenv:Body>
</soapenv:Envelope>";

            string xml = string.Format(format, key);
            return xml;
        }


        private static List<LookupValue> GetPicklistValues(string soapEnvelope)
        {
            // TODO: handle SOAP faults

            // see https://stackoverflow.com/questions/5185389/creating-xslt-transform-to-flatten-multiref-encoded-soap-message
            // flatten multiRef encoded SOAP message

            string stylesheet = @"
<xsl:stylesheet version=""1.0""
                xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""
                xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" >
    <xsl:key name=""multiref-by-id"" match=""multiRef"" use=""@id""/>
    <xsl:template match=""/"">
        <xsl:copy>
            <xsl:apply-templates select=""@*|*""/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match=""*[starts-with(@href, '#')]"">
        <xsl:copy>
            <xsl:apply-templates select=""@* |
             key('multiref-by-id', substring-after(@href, '#'))/@* |
            key('multiref-by-id', substring-after(@href, '#'))/node()""/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match=""@href[starts-with(., '#')] | multiRef[@id] | @soapenc:root""/>
    <xsl:template match=""@*|node()"">
        <xsl:copy>
            <xsl:apply-templates select=""@*|node()""/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>";

            XslCompiledTransform GetXslt()
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                using StringReader sr = new StringReader(stylesheet);
                using XmlReader xr = XmlReader.Create(sr);
                xslt.Load(xr);

                return xslt;
            }

            string TransformXslt(XslCompiledTransform xslt, string xml)
            {
                using StringReader sr = new StringReader(xml);
                using XmlReader xr = XmlReader.Create(sr);
                using StringWriter sw = new StringWriter();

                xslt.Transform(xr, null, sw);

                return sw.ToString();
            }

            List<LookupValue> ExtractPicklistValues(string xml)
            {
                var document = XDocument.Parse(xml);

                // get the <getPicklistKeywordsResponse> element inside the body
                var getPicklistKeywordsResponse = (XElement)document.Document
                    .Elements(XName.Get(@"{http://schemas.xmlsoap.org/soap/envelope/}Envelope"))
                    .Elements(XName.Get(@"{http://schemas.xmlsoap.org/soap/envelope/}Body"))
                    .Single()
                    .FirstNode;

                var getPicklistKeywordsReturnWrapper = (XElement)getPicklistKeywordsResponse.FirstNode;

                LookupValue GetPicklistItem(XElement getPicklistKeywordsReturn)
                {
                    /*
                     <getPicklistKeywordsReturn>
                       <item></item>
                       <item></item>
                     </getPicklistKeywordsReturn>
                     */

                    LookupValue lookupValue = new LookupValue();

                    foreach (var item in getPicklistKeywordsReturn.Elements())
                    {
                        string itemKey = string.Empty;
                        string itemValue = string.Empty;

                        /*
                               <item xmlns:ns28="http://xml.apache.org/xml-soap">
                                  <key xsi:type="soapenc:string">id</key>
                                  <value xsi:type="soapenc:string">60130</value>
                               </item>
                         */
                        foreach (var element in item.Elements())
                        {
                            if (element.Name == "key")
                            {
                                itemKey = element.Value;
                            }
                            else if (element.Name == "value")
                            {
                                itemValue = element.Value;
                            }
                        }

                        switch (itemKey)
                        {
                            case "encodedValue":
                                break;
                            case "id":
                                lookupValue.Id = itemValue;
                                break;
                            case "val":
                                break;
                            case "value":
                                lookupValue.Value = itemValue;
                                break;
                        }

                    }

                    return lookupValue;
                }


                var items = getPicklistKeywordsReturnWrapper
                    .Elements()
                    .Select(GetPicklistItem)
                    .ToList();

                return items;
            }


            string transformed = TransformXslt(GetXslt(), soapEnvelope);
            return ExtractPicklistValues(transformed);
        }
    }
}
