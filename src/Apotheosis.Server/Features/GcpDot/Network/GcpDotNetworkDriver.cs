﻿using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Features.GcpDot.Interfaces;
using Apotheosis.Server.Features.GcpDot.Configuration;
using Apotheosis.Server.Features.GcpDot.Exceptions;
using Newtonsoft.Json;

namespace Apotheosis.Server.Features.GcpDot.Network;

public sealed class GcpDotNetworkDriver : IGcpDotNetworkDriver
{
    readonly HttpClient _httpClient;

    /// <summary>
    /// Gets the number of times to retry requests to GCP Dot.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the GCP Dot service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public GcpDotNetworkDriver(HttpClient httpClient, IOptions<GcpDotSettings> gcpDotOptions)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = gcpDotOptions.Value.BaseUrl;
    }

    public async Task<string> SendRequestAsync(string path, HttpMethod method, object? request)
    {
        using var requestMessage = new HttpRequestMessage(method, path);

        if (request != null)
        {
            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            requestMessage.Content = content;
        }

        using var response = await _httpClient.SendAsync(requestMessage);

        var data = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new GcpDotNetworkException("GCP Dot service returned a non-successful status code", null);
        }

        return data;
    }
}