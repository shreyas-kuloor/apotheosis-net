using System.Diagnostics;

namespace Apotheosis.Components.Audio.Interfaces;

public interface IAudioService
{
    Task<Stream> ConvertMp3ToPcmAsync(Stream mp3Stream);

    Task<Process> CreateStreamProcessAsync(Stream pcmStream);
}