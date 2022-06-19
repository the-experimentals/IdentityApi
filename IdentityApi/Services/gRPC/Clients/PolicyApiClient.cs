using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using IdentityApi.ResponseModels;
using Microsoft.AspNetCore.Http;

namespace IdentityApi.Services.gRPC.Clients;

public class PolicyApiClient
{
    private readonly PolicyApi.Protos.PolicyApi.PolicyApiClient _client;
    private readonly IMapper _mapper;
    private readonly string _serviceURL = "http://localhost:6001";

    public PolicyApiClient(IMapper mapper, IHttpContextAccessor httpContext)
    {
        _mapper = mapper;

        GrpcWebHandler handler = new(GrpcWebMode.GrpcWebText, new HttpClientHandler());

        var channel =
            GrpcChannel.ForAddress(_serviceURL, new GrpcChannelOptions { HttpClient = new HttpClient(handler) });

        _client = new PolicyApi.Protos.PolicyApi.PolicyApiClient(channel);
    }

    public async Task<TokenResponse> GetPolicyTokenAsync(Metadata header)
    {
        PolicyApi.Protos.TokenResponse tokenResponse = null;

        try
        {
            tokenResponse = await _client.GetPolicyTokenAsync(new Empty(), header);
        }
        catch (RpcException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.InnerException);
        }

        return _mapper.Map<TokenResponse>(tokenResponse);
    }
}
