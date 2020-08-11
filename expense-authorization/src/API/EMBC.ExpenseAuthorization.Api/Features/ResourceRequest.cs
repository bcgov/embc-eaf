using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
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
            private readonly IETeamRestService _eteamsService;
            private readonly IOptions<ETeamSettings> _options;
            private readonly IEmailSender _emailSender;

            public Handler(IETeamRestService eteamsService, IOptions<ETeamSettings> options, IEmailSender emailSender)
            {
                _eteamsService = eteamsService ?? throw new ArgumentNullException(nameof(eteamsService));
                _options = options ?? throw new ArgumentNullException(nameof(options));
                _emailSender = emailSender;
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

                var response = await _eteamsService.CreateReportAsync(username, password, reportTypeName, request.Request);

                if (request.Files != null && request.Files.Count != 0)
                {

                    var message = new Message
                    {
                        To = new List<MailboxAddress> { new MailboxAddress("philbolduc@gmail.com")},
                        Attachments = request.Files,
                        Content = "Attachments",
                        Subject = "E-Team Resource Request"
                    };

                    await _emailSender.SendEmailAsync(message);

                }

                return new CreateResponse();
            }
        }
    }
}
