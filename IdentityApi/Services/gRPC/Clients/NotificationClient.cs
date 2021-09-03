using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using IdentityApi.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using static IdentityApi.Protos.NotificationApi;

namespace IdentityApi.Services.gRPC.Clients
{
    public class NotificationClient
    {
        private readonly NotificationApiClient _client;
        private readonly IMapper _mapper;
        private readonly string _serviceURL = "http://localhost:6002";

        public NotificationClient(IMapper mapper, IHttpContextAccessor httpContext)
        {
            _mapper = mapper;

            var headers = new Metadata();
            var token = httpContext.HttpContext.Request.Headers[HeaderNames.Authorization];
            headers.Add("Authorization", $"{token}");

            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(token))
                {
                    metadata.Add("Authorization", $"{token}");
                }

                return Task.CompletedTask;
            });

            GrpcWebHandler handler = new(GrpcWebMode.GrpcWebText, new HttpClientHandler());

            var channel = GrpcChannel.ForAddress(_serviceURL, new()
            {
                HttpClient = new(handler)
                //Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

            _client = new(channel);
        }

        public void SendEmail(EmailRequest emailRequest, Metadata header)
        {
            var response = _client.SendEmailAsync(_mapper.Map<IdentityApi.Protos.EmailRequest>(emailRequest));
        }
    }
}
