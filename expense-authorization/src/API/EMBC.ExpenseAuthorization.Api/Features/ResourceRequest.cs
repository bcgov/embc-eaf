using System;
using System.Threading;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace EMBC.ExpenseAuthorization.Api.Features
{
    public class ResourceRequest
    {
        public class CreateCommand : IRequest<CreateResponse>
        {
            public ResourceRequestModel Request { get; }

            public CreateCommand(ResourceRequestModel request)
            {
                Request = request ?? throw new ArgumentNullException(nameof(request));
            }
        }

        public class CreateResponse
        {
        }
        
        public class Handler : IRequestHandler<CreateCommand, CreateResponse>
        {
            private readonly IETeamRestService _eteamsService;
            private readonly IOptions<ETeamSettings> _options;

            public Handler(IETeamRestService eteamsService, IOptions<ETeamSettings> options)
            {
                _eteamsService = eteamsService ?? throw new ArgumentNullException(nameof(eteamsService));
                _options = options ?? throw new ArgumentNullException(nameof(options));
            }

            public async Task<CreateResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var settings = _options.Value;

                string username = settings.Username;
                string password = settings.Password;
                string reportTypeName = settings.ReportTypeName;

                await _eteamsService.CreateReportAsync(username, password, reportTypeName, request.Request);

                return new CreateResponse();
            }
        }
    }
}
