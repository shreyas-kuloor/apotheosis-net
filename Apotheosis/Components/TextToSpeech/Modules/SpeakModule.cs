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

    [SlashCommand("speak", "Invite the bot to speak a prompt using a voice ID of your choice.")]
    public async Task SpeakAsync(string voiceId, string prompt)
    {
        if (Context.Guild.CurrentUser.IsStreaming)
        {
            await RespondAsync("I'm currently speaking in a voice channel, please try again after I'm done!");
        }
        
        var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel;
        
        var voiceMp3Stream = await _textToSpeechService.GenerateSpeechFromPromptAsync(prompt, voiceId);

        if (voiceChannel == null)
        {
            await Context.Interaction.RespondWithFileAsync(voiceMp3Stream, $"{voiceId}_speech.mp3");
            return;
        }

        var audioClient = await voiceChannel.ConnectAsync();
        var voicePcmStream = await _audioService.ConvertMp3ToPcmAsync(voiceMp3Stream);

        using var ffmpeg = await _audioService.CreateStreamProcessAsync(voicePcmStream);
        await using var stream = audioClient.CreatePCMStream(AudioApplication.Music);
        try
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        }
        finally
        {
            await stream.FlushAsync();
        }
    }
}