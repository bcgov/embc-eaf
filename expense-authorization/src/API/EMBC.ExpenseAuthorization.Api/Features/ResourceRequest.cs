using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EMBC.ExpenseAuthorization.Api.Features
{
    public class ResourceRequest
    {
        public class CreateCommand : IRequest<CreateResponse>
        {
            public ResourceRequestModel Request { get; }
            public IList<IFormFile> Files { get; } = Array.Empty<IFormFile>();

            public CreateCommand(ResourceRequestModel request, IEnumerable<IFormFile> files)
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
        }
        
        public class Handler : IRequestHandler<CreateCommand, CreateResponse>
        {
            private readonly IETeamSoapService _eteamService;
            private readonly IEmailService _emailService;
            
            public Handler(IETeamSoapService eteamService, IEmailService emailService)
            {
                _eteamService = eteamService ?? throw new ArgumentNullException(nameof(eteamService));
                _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            }

            public async Task<CreateResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }
                
                CreateReportResponse response = await _eteamService.CreateReportAsync(request.Request);

                await _emailService.SendEmailAsync(response, request.Files);
                
                return new CreateResponse();
            }
        }
    }
}
