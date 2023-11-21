using Apotheosis.Components.Audio.Interfaces;
using Apotheosis.Components.TextToSpeech.Interfaces;
using Discord;
using Discord.Audio;
using Discord.Interactions;

namespace Apotheosis.Components.TextToSpeech.Modules;

public class SpeakModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;
    private readonly ITextToSpeechService _textToSpeechService;

    public SpeakModule(
        IAudioService audioService,
        ITextToSpeechService textToSpeechService)
    {
        _audioService = audioService;
        _textToSpeechService = textToSpeechService;
    }

    [SlashCommand("speak", "Invite the bot to speak a prompt using a voice of your choice.")]
    public async Task SpeakAsync(string voiceName, string prompt)
    {
        if (Context.Guild.CurrentUser.IsStreaming)
        {
            await RespondAsync("I'm currently speaking in a voice channel, please try again after I'm done!", ephemeral: true);
        }
        
        var voices = await _textToSpeechService.GetVoicesAsync();
        var voiceId = voices.FirstOrDefault(v => v.VoiceName?.Equals(voiceName, StringComparison.OrdinalIgnoreCase) ?? false)?.VoiceId;

        if (string.IsNullOrWhiteSpace(voiceId))
        {
            await RespondAsync(
                $"{voiceName} is not valid. Please try again with a valid voice name. A list of voices can be retrieved with the /voices command", ephemeral: true);
        }
        
        var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel;
        await DeferAsync(ephemeral: true);
        var voiceMp3Stream = await _textToSpeechService.GenerateSpeechFromPromptAsync(prompt, voiceId!);

        if (voiceChannel == null)
        {
            await FollowupAsync(
                $"Generated speech with prompt \"{prompt}\" using voice \"{voiceName}\"",
                ephemeral: true);
            await Context.Interaction.FollowupWithFileAsync(
                voiceMp3Stream, 
                $"{voiceName}_speech.mp3",
                ephemeral: false);
            return;
        }

        
        var audioClient = await voiceChannel.ConnectAsync();
        using var ffmpeg = await _audioService.CreateStreamProcessAsync(voiceMp3Stream);
        await using var stream = audioClient.CreatePCMStream(AudioApplication.Music);
        try
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        }
        finally
        {
            await stream.FlushAsync();
            await voiceChannel.DisconnectAsync();
            await FollowupAsync(
                $"Generated speech with prompt \"{prompt}\" using voice \"{voiceName}\"", 
                ephemeral: true);
        }
    }
    
    [SlashCommand("voices", "Get the list of all supported voices for the /speak command.")]
    public async Task GetVoicesAsync()
    {
        var voices = await _textToSpeechService.GetVoicesAsync();

        var voicesString = $"**Available Voices:** {string.Join(", ", voices.Select(v => v.VoiceName))}";
        await Context.Interaction.RespondAsync(voicesString, ephemeral: true);
    }
}