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
            private readonly IEmailSender _emailSender;

            public Handler(IETeamSoapService eteamService, IEmailSender emailSender)
            {
                _eteamService = eteamService ?? throw new ArgumentNullException(nameof(eteamService));
                _emailSender = emailSender;
            }

            public async Task<CreateResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }


                CreateReportResponse response = await _eteamService.CreateReportAsync(request.Request);

                // seems the output has the id in reportId / id
                if (!response.Fields.TryGetValue("reportId", out var reportId))
                {
                    response.Fields.TryGetValue("id", out reportId);
                }

                if (request.Files != null && request.Files.Count != 0)
                {

                    var message = new Message
                    {
                        To = new List<MailboxAddress> { new MailboxAddress("philbolduc@gmail.com")},
                        Attachments = request.Files,
                        Content = $"Attachments for Report: {reportId}",
                        Subject = "E-Team Resource Request",
                    };

                    await _emailSender.SendEmailAsync(message);

                }

                return new CreateResponse();
            }
        }
    }
}
