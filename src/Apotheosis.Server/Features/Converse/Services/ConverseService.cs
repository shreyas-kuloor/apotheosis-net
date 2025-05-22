using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.Converse.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;

namespace Apotheosis.Server.Features.Converse.Services;

[Scoped]
public sealed class ConverseService(
    IAiChatService aiChatService,
    ITextToSpeechService textToSpeechService)
    : IConverseService
{
    public async Task<Stream> GenerateConverseResponseFromPromptAsync(string prompt, string voiceName, string voiceId, CancellationToken cancellationToken)
    {
        var aiChatResponse = await aiChatService.GetChatResponseAsync(prompt, cancellationToken);

        var voiceStream = await textToSpeechService.GenerateSpeechFromPromptAsync(aiChatResponse, voiceId);

        return voiceStream;
    }
}