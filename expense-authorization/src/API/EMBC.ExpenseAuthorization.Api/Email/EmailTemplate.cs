using System;
using System.Collections.Generic;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public class EmailTemplate
    {
        public const string DateFormat = "MMMM dd, yyyy HH:mm";

        public EmailTemplate()
        {
            Content = EmbeddedResource.Get<EmailService>("email_template.html");
        }

        /// <summary>
        /// Gets the email template content.
        /// </summary>
        public string Content { get; private set; }
        
        public EmailTemplate Apply(ExpenseAuthorizationRequest data)
        {
            Apply(data, "Request.");
            return this;
        }

        public EmailTemplate Apply(CreateReportResponse response, Uri baseUri)
        {
            var baseUrl = baseUri.ToString();
            if (baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = baseUrl.TrimEnd('/');
            }

            string reportUrl;

            if (response.Fields.ContainsKey("id"))
            {
                // https://host/instance/report/resource.do?target=read&id=id&reportType=resource_request
                reportUrl = baseUrl + "/report/resource.do?target=read&reportType=resource_request&id=" + response.Fields["id"];
            }
            else
            {
                // that is not good
                reportUrl = baseUrl;
            }

            Apply(reportUrl, "Report.Url");
            Apply(response, "Response.");
            return this;
        }

        private void Apply(object value, string placeHolderPrefix)
        {
            if (value == null)
            {
                return;
            }

            if (value is IDictionary<string, string>)
            {
                // avoid trying to apply a dictionary
                return;
            }

            if (value is Uri valueAsUri)
            {
                Content = Content.Replace(FormatPlaceholder(placeHolderPrefix), valueAsUri.ToString());
                return;
            }
            if (value is string valueAsString)
            {
                Content = Content.Replace(FormatPlaceholder(placeHolderPrefix), valueAsString);
                return;
            }

            var properties = value.GetType().GetProperties();

            foreach (var property in properties)
            {
                object propertyValue = property.GetValue(value);
                if (propertyValue == null)
                {
                    continue;
                }

                string stringValue = null;

                if (property.PropertyType == typeof(string))
                {
                    stringValue = (string)propertyValue;
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    stringValue = ((DateTime)propertyValue).ToString(DateFormat);
                }
                else if (property.PropertyType == typeof(int))
                {
                    stringValue = propertyValue.ToString();
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    stringValue = propertyValue.ToString();
                }
                else
                {
                    // nested object
                    Apply(propertyValue, placeHolderPrefix + property.Name + ".");
                }

                if (stringValue != null)
                {
                    string placeholder = FormatPlaceholder(placeHolderPrefix + property.Name);
                    Content = Content.Replace(placeholder, stringValue);
                }
            }
        }


        private string FormatPlaceholder(string name) => "{{" + name + "}}";
    }
}
