using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using PolicyApi.Protos;

namespace IdentityApi.Services.gRPC.Clients
{
    public class PolicyApiClient
    {
        private readonly PolicyApi.Protos.PolicyApi.PolicyApiClient _client;
        private readonly IMapper _mapper;
        private readonly string _serviceURL = "http://localhost:6002";

        public PolicyApiClient(IMapper mapper, IHttpContextAccessor httpContext)
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

        public async Task<ResponseModels.TokenResponse> GetPolicyTokenAsync(Metadata header)
        {
            TokenResponse tokenResponse = null;

            try
            {
                tokenResponse = await _client.GetPolicyTokenAsync(new Google.Protobuf.WellKnownTypes.Empty(), header);
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.InnerException);
            }

            return _mapper.Map<ResponseModels.TokenResponse>(tokenResponse);
        }
    }
}
