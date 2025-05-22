using System.Diagnostics;
using Apotheosis.Core.Features.Audio.Interfaces;

namespace Apotheosis.Server.Features.Audio.Services;

[Scoped]
public sealed class AudioStreamService : IAudioStreamService
{
    public Process? CreateStreamProcessAsync()
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = "-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
        }
        );
    }
}