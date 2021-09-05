using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using IdentityApi.Protos;
using Microsoft.AspNetCore.Http;
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
            
            GrpcWebHandler handler = new(GrpcWebMode.GrpcWebText, new HttpClientHandler());

            var channel = GrpcChannel.ForAddress(_serviceURL, new()
            {
                HttpClient = new(handler)                
            });

            _client = new(channel);
        }

        public async Task<EmailResponse> SendEmailAsync(RequestModels.EmailRequest emailRequest, Metadata header)
        {
           return await _client.SendEmailAsync(_mapper.Map<Protos.EmailRequest>(emailRequest), header);
        }
    }
}
