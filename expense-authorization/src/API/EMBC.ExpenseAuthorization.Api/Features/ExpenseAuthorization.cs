using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EMBC.ExpenseAuthorization.Api.Features
{
    public class ExpenseAuthorization
    {
        public class CreateCommand : IRequest<CreateResponse>
        {
            public ExpenseAuthorizationRequest Request { get; }
            public IList<IFormFile> Files { get; } = Array.Empty<IFormFile>();

            public CreateCommand(ExpenseAuthorizationRequest request, IEnumerable<IFormFile> files)
            {
                Request = request ?? throw new ArgumentNullException(nameof(request));

                if (files != null)
                {
                    Files = new List<IFormFile>(files);
                }
            }
        }

        public class CreateResponse
        {
            public CreateResponse(string id)
            {
                Id = id;
            }

            public CreateResponse(Exception exception)
            {
                Exception = exception;
            }

            public string Id { get; set; }
            
            public Exception Exception { get; set; }
        }
        
        public class Handler : IRequestHandler<CreateCommand, CreateResponse>
        {
            private readonly IETeamSoapService _eteamService;
            private readonly IEmailService _emailService;
            private readonly ILogger<Handler> _logger;

            public Handler(IETeamSoapService eteamService, IEmailService emailService, ILogger<Handler> logger)
            {
                _eteamService = eteamService ?? throw new ArgumentNullException(nameof(eteamService));
                _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<CreateResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var loggingState = new Dictionary<string, object>
                {
                    {"ExpenseAuthorization.Request", request.Request},
                    {"ExpenseAuthorization.FileCount", request.Files.Count },
                };

                // ensure we log the request details, especially if an error occurs
                using (_logger.BeginScope(loggingState))
                {
                    try
                    {
                        _logger.LogDebug("Creating report in E-Team");
                        CreateReportResponse response = await _eteamService.CreateReportAsync(request.Request);

                        _logger.LogDebug("Sending email to notify of new request submission");
                        await _emailService.SendEmailAsync(request.Request, response, request.Files);

                        return new CreateResponse(response.Fields["id"]);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to create expense authorization");
                        return new CreateResponse(e);
                    }
                }
            }
        }
    }
}
