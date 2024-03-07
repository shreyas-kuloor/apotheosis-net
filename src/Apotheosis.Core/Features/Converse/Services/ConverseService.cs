using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.Converse.Configuration;
using Apotheosis.Core.Features.Converse.Interfaces;
using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;

namespace Apotheosis.Core.Features.Converse.Services;

[Scoped]
public sealed class ConverseService(
    IAiChatService aiChatService,
    ITextToSpeechService textToSpeechService,
    ILogService<ConverseService> logger,
    IOptions<ConverseSettings> converseOptions)
    : IConverseService
{
    readonly ConverseSettings _converseSettings = converseOptions.Value;

    public async Task<Stream> GenerateConverseResponseFromPromptAsync(string prompt, string voiceName, string voiceId)
    {
        var supportedVoice =
            _converseSettings.VoiceSystemInstructions!.TryGetValue(voiceName.ToLowerInvariant(), out var systemPrompt);

        if (!supportedVoice)
        {
            _converseSettings.VoiceSystemInstructions!.TryGetValue("default", out systemPrompt);
        }

        if (string.IsNullOrEmpty(systemPrompt))
        {
            logger.LogWarning(null, "Default voice system instruction for converse operation not found.");
        }

        var aiChatResponses = await aiChatService.SendSingleMessageToAiAsync(prompt, systemPrompt);
        var singleResponse = aiChatResponses.First(r => r.Index == 0);

        var voiceStream = await textToSpeechService.GenerateSpeechFromPromptAsync(singleResponse.Content!, voiceId);

        return voiceStream;
    }
}