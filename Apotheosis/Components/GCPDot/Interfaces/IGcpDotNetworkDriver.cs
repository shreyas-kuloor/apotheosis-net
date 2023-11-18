namespace Apotheosis.Components.GCPDot.Interfaces;

public interface IGcpDotNetworkDriver
{
    Task<string> SendRequestAsync(string path, HttpMethod method, object? request);
}