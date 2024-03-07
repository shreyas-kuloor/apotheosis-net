using System.Diagnostics;

namespace Apotheosis.Core.Features.Audio.Interfaces;

public interface IAudioStreamService
{
    Process? CreateStreamProcessAsync();
}