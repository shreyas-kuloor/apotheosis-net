using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Features.AiChat.Configuration;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.AiChat.Interfaces;

namespace Apotheosis.Core.Features.AiChat.Network;

public sealed class AiChatClient(HttpClient httpClient, IOptions<AiChatSettings> aiChatOptions)
    : IAiChatClient
{
    readonly AiChatSettings _aiChatSettings = aiChatOptions.Value;

    /// <summary>
    /// Gets the number of times to retry requests to AI Chat service.
    /// </summary>
    public const int Retries = 5;

    /// <summary>
    /// Gets the number of seconds to wait until a timeout from the AI Chat service (in seconds).
    /// </summary>
    public const int Timeout = 10;

    public async Task<string> GetChatResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _aiChatSettings.Url);
        var content = new StringContent(prompt, Encoding.UTF8, MediaTypeNames.Text.Plain);
        requestMessage.Content = content;

        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        var data = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AiChatNetworkException("Chat returned a non-successful status code");
        }

        return data;
    }
}