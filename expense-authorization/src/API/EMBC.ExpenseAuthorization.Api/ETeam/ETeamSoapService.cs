using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Requests;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Microsoft.Extensions.Options;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public class ETeamSoapService : IETeamSoapService
    {
        private const string DefaultResourceCategory = "Expenditure Authorization";
        private const string DefaultCurrentStatus = "Black-New Request";
        private const string ExpenditureAuthorizationResourceTypePrefix = "Expenditure Authorization-";

        private readonly IETeamSoapClient _client;
        private readonly IOptions<ETeamSettings> _options;

        /// <summary>Initializes a new instance of the <see cref="ETeamSoapService" /> class.</summary>
        /// <param name="client">The client.</param>
        /// <param name="options">The E Team configuration options</param>
        /// <exception cref="ArgumentNullException">client</exception>
        public ETeamSoapService(IETeamSoapClient client, IOptions<ETeamSettings> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IList<LookupValue>> GetExpenditureAuthorizationResourceTypesAsync()
        {
            // Get all the resource types. Resource type descriptions are prefixed with the resource category 
            IList<LookupValue> resourceTypeValues = await GetPicklistKeywordsAsync(LookupType.ResourceType);

            IList<LookupValue> values = resourceTypeValues
                .Where(_ => _.Value != null && _.Value.StartsWith(ExpenditureAuthorizationResourceTypePrefix))
                .Select(_ => new LookupValue { Id = _.Id, Value = _.Value.Substring(ExpenditureAuthorizationResourceTypePrefix.Length)})
                .ToList();

            return values;
        }

        /// <summary>Gets the lookup asynchronous.</summary>
        /// <param name="lookupType">Type of the lookup.</param>
        /// <returns></returns>
        public async Task<IList<LookupValue>> GetLookupAsync(LookupType lookupType)
        {
            IList<LookupValue> values;

            switch (lookupType)
            {
                case LookupType.PriorityResource:
                case LookupType.StatusResource:
                    values = await GetPicklistColorsAsync(lookupType);
                    break;

                default:
                    values = await GetPicklistKeywordsAsync(lookupType);
                    break;
            }

            return values;

        }

        public async Task<CreateReportResponse> CreateReportAsync(ResourceRequestModel resourceRequest)
        {
            if (resourceRequest == null)
            {
                throw new ArgumentNullException(nameof(resourceRequest));
            }

            ETeamSettings settings = _options.Value;

            // login and get session cookie
            await LoginAsync(settings);

            CreateReportResponse response = await CreateReportAsync(settings, resourceRequest);

            return response;
        }

        private async Task<IList<LookupValue>> GetPicklistColorsAsync(LookupType lookupType)
        {
            var request = new GetPicklistColorsRequest(lookupType);

            // create the correct SOAP Envelope for the request
            string soapRequest = request.CreateSoapRequest();

            // make SOAP request and get the response SOAP Envelope
            string soapResponse = await _client.GetPicklistColorsAsync(soapRequest);

            GetPicklistColorsResponse response = new GetPicklistColorsResponse();
            response.LoadFromXml(soapResponse);

            return response.Values;
        }

        /// <summary>
        /// Gets the pick list lookup.
        /// </summary>
        /// <param name="lookupType"></param>
        /// <returns></returns>
        /// <exception cref="ApiException">
        /// Error calling SOAP service.  See <see cref="ApiException.Content"/> for more information.
        /// </exception>
        private async Task<IList<LookupValue>> GetPicklistKeywordsAsync(LookupType lookupType)
        {
            var request = new GetPicklistKeywordsRequest(lookupType);

            // create the correct SOAP Envelope for the request
            string soapRequest = request.CreateSoapRequest();

            try
            {
                // make SOAP request and get the response SOAP Envelope
                string soapResponse = await _client.GetPicklistKeywordsAsync(soapRequest);

                GetPicklistKeywordsResponse response = new GetPicklistKeywordsResponse();
                response.LoadFromXml(soapResponse);
                return response.Values;
            }
            catch (ApiException e)
            {
                throw new SoapFaultException(e);
            }
        }

        private async Task<CreateReportResponse> CreateReportAsync(ETeamSettings settings, ResourceRequestModel resourceRequest)
        {
            // get the defaults, we could cache this in the future
            var resourceCategories = await GetLookupAsync(LookupType.ResourceCategory);
            var statuses = await GetLookupAsync(LookupType.StatusResource);
            var priorities = await GetLookupAsync(LookupType.PriorityResource);

            resourceRequest.ResourceCategory = resourceCategories.FirstOrDefault(_ => _.Value == DefaultResourceCategory)?.Value;
            resourceRequest.CurrentStatus = statuses.FirstOrDefault(_ => _.Value == DefaultCurrentStatus)?.Value;
            resourceRequest.Priority = priorities.FirstOrDefault(_ => _.Value == "Green-Routine")?.Value;

            if (string.IsNullOrEmpty(resourceRequest.RequestorContactInfo))
            {
                resourceRequest.RequestorContactInfo = "place holder due to code not finished";
            }

            var soapRequest = GetCreateReportSoapRequest(settings.ReportTypeName, resourceRequest);

            try
            {
                var soapResponse = await _client.CreateReportAsync(soapRequest);

                CreateReportResponse response = new CreateReportResponse();
                response.LoadFromXml(soapResponse);

                return response;
            }
            catch (ApiException exception)
            {
                throw new SoapFaultException(exception);
            }
        }

        private async Task LoginAsync(ETeamSettings settings)
        {
            LoginRequest loginRequest = new LoginRequest(settings.Username, settings.Password);
            string soapRequest = loginRequest.CreateSoapRequest();

            try
            {
                string soapResponse = await _client.LoginAsync(soapRequest);
                LoginResponse loginResponse = new LoginResponse();
                loginResponse.LoadFromXml(soapResponse);
            }
            catch (ApiException exception)
            {
                throw new SoapFaultException(exception);
            }
        }

        private static string GetCreateReportSoapRequest(string reportTypeName, ResourceRequestModel resourceRequest)
        {
            var items = new Dictionary<string, string>();

            AddIfNotNullOrEmpty(items, "reportType", reportTypeName);

            // The items below that are commented out are defined in ETeam but are not used 
            // by this application. They have been left here in case they need to be added in the future.
            AddIfNotNullOrEmpty(items, "approvedBy", resourceRequest.ApprovedBy);
            AddIfNotDefault(items, "approvedTime", resourceRequest.ApprovedTime);
            AddIfNotNullOrEmpty(items, "currentStatus", resourceRequest.CurrentStatus);
            Add(items, "estimatedResourceCost", resourceRequest.EstimatedResourceCost);
            AddIfNotNullOrEmpty(items, "mission", resourceRequest.Mission);
            ////AddIfNotNullOrEmpty(items, "mustComeWithFuel", resourceRequest.MustComeWithFuel);
            ////AddIfNotNullOrEmpty(items, "mustComeWithLodging", resourceRequest.MustComeWithLodging);
            ////AddIfNotNullOrEmpty(items, "mustComeWithMaint", resourceRequest.MustComeWithMaint);
            ////AddIfNotNullOrEmpty(items, "mustComeWithMeals", resourceRequest.MustComeWithMeals);
            ////AddIfNotNullOrEmpty(items, "mustComeWithOperator", resourceRequest.MustComeWithOperator);
            ////AddIfNotNullOrEmpty(items, "mustComeWithOther", resourceRequest.MustComeWithOther);
            ////AddIfNotNullOrEmpty(items, "mustComeWithPower", resourceRequest.MustComeWithPower);
            ////AddIfNotNullOrEmpty(items, "mustComeWithWater", resourceRequest.MustComeWithWater);
            AddIfNotNullOrEmpty(items, "priority", resourceRequest.Priority);
            ////AddIfNotNullOrEmpty(items, "qtyUnitOfMeasurement", resourceRequest.QtyUnitOfMeasurement);
            items.Add("quantity", "1");
            AddIfNotNullOrEmpty(items, "reqTrackNoEmac", resourceRequest.ReqTrackNoEmac);
            AddIfNotNullOrEmpty(items, "reqTrackNoFema", resourceRequest.ReqTrackNoFema);
            AddIfNotNullOrEmpty(items, "reqTrackNoState", resourceRequest.ReqTrackNoState);
            AddIfNotNullOrEmpty(items, "requestNumber", resourceRequest.RequestNumber);
            AddIfNotNullOrEmpty(items, "requestionOrg", resourceRequest.RequestionOrg);
            AddIfNotNullOrEmpty(items, "requestorContactInfo", resourceRequest.RequestorContactInfo);
            AddIfNotNullOrEmpty(items, "resourceCategory", resourceRequest.ResourceCategory);
            AddIfNotNullOrEmpty(items, "resourceType", resourceRequest.ResourceType);
            ////AddIfNotNullOrEmpty(items, "resourceTypeTemp", resourceRequest.ResourceTypeTemp);
            ////AddIfNotNullOrEmpty(items, "specialInstructions", resourceRequest.SpecialInstructions);
            ////AddIfNotNullOrEmpty(items, "summaryOfActionsTaken", resourceRequest.SummaryOfActionsTaken);
            AddIfNotDefault(items, "whenNeeded", resourceRequest.WhenNeeded);

            var request = new CreaterReportRequest(items);
            string soapRequest = request.CreateSoapRequest();

            return soapRequest;
        }

        private static void AddIfNotNullOrEmpty(Dictionary<string, string> items, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                items.Add(name, value);
            }
        }

        private static void AddIfNotDefault(Dictionary<string, string> items, string name, DateTime value)
        {
            if (value != default)
            {
                items.Add(name, value.ToString("o"));
            }
        }

        private static void Add(Dictionary<string, string> items, string name, int value)
        {
            items.Add(name, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
