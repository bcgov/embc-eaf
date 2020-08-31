using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Requests;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using Microsoft.Extensions.Options;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam
{
    public class ETeamSoapService : IETeamSoapService
    {
        private const string DefaultResourceCategory = "Expenditure Authorization";
        private const string DefaultCurrentStatus = "Black-New Request";
        private const string DefaultPriority = "Green-Routine";
        private const string ExpenditureAuthorizationResourceTypePrefix = "Expenditure Authorization-";

        private readonly IETeamSoapClient _client;
        private readonly IOptions<ETeamSettings> _options;
        private readonly IExpenseAuthorizationRequestMapper _mapper;

        /// <summary>Initializes a new instance of the <see cref="ETeamSoapService" /> class.</summary>
        /// <param name="client">The client.</param>
        /// <param name="options">The E Team configuration options</param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException">client</exception>
        public ETeamSoapService(IETeamSoapClient client, IOptions<ETeamSettings> options, IExpenseAuthorizationRequestMapper mapper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _mapper = mapper;
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

        public async Task<CreateReportResponse> CreateReportAsync(ExpenseAuthorizationRequest expenseAuthorizationRequest)
        {
            if (expenseAuthorizationRequest == null)
            {
                throw new ArgumentNullException(nameof(expenseAuthorizationRequest));
            }

            ETeamSettings settings = _options.Value;

            // login and get session cookie
            await LoginAsync(settings);

            CreateReportResponse response = await CreateReportAsync(settings, expenseAuthorizationRequest);

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

        private async Task<CreateReportResponse> CreateReportAsync(ETeamSettings settings, ExpenseAuthorizationRequest expenseAuthorizationRequest)
        {
            // get the defaults, we could cache this in the future
            var resourceCategories = await GetLookupAsync(LookupType.ResourceCategory);
            var statuses = await GetLookupAsync(LookupType.StatusResource);
            var priorities = await GetLookupAsync(LookupType.PriorityResource);

            string resourceCategory = resourceCategories.FirstOrDefault(_ => _.Value == DefaultResourceCategory)?.Value;
            string currentStatus = statuses.FirstOrDefault(_ => _.Value == DefaultCurrentStatus)?.Id;
            string priority = priorities.FirstOrDefault(_ => _.Value == DefaultPriority)?.Id;

            var items = _mapper.Map(expenseAuthorizationRequest, priority, resourceCategory, currentStatus);

            var request = new CreaterReportRequest(items);
            string soapRequest = request.CreateSoapRequest();

            try
            {
                CreateReportResponse response = new CreateReportResponse();

#if true
                var soapResponse = await _client.CreateReportAsync(soapRequest);
                response.LoadFromXml(soapResponse);
#else
                response.Fields["id"] = Guid.NewGuid().ToString("n");
#endif

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
    }
}
