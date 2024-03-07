namespace Apotheosis.Core.Features.GcpDot.Interfaces;

public interface IGcpDotNetworkDriver
{
    Task<string> SendRequestAsync(string path, HttpMethod method, object? request);
}