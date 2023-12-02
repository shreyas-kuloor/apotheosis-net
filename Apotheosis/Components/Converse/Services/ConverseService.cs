using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.Converse.Configuration;
using Apotheosis.Components.Converse.Interfaces;
using Apotheosis.Components.Logging.Interfaces;
using Apotheosis.Components.TextToSpeech.Interfaces;

namespace Apotheosis.Components.Converse.Services;

public sealed class ConverseService(
    IAiChatService aiChatService,
    ITextToSpeechService textToSpeechService,
    ILogService<ConverseService> logger,
    ConverseSettings converseSettings)
    : IConverseService
{
    public async Task<Stream> GenerateConverseResponseFromPromptAsync(string prompt, string voiceName, string voiceId)
    {
        var supportedVoice =
            converseSettings.VoiceSystemInstructions!.TryGetValue(voiceName.ToLowerInvariant(), out var systemPrompt);

        if (!supportedVoice)
        {
            converseSettings.VoiceSystemInstructions!.TryGetValue("default", out systemPrompt);
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