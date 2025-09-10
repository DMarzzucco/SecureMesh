using System;
using Grpc.Core;

namespace UserDomainTest.Helper;

public class TestServerCallContext : ServerCallContext
{
    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options) => throw new NotImplementedException();
    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
    protected override string MethodCore => "test";
    protected override string HostCore => "localhost";
    protected override string PeerCore => "test";
    protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);
    protected override Metadata RequestHeadersCore => [];
    protected override CancellationToken CancellationTokenCore => CancellationToken.None;
    protected override Metadata ResponseTrailersCore => [];
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore => new("anonymous", []);
    public static ServerCallContext Create() => new TestServerCallContext();
}
