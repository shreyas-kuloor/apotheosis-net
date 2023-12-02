using System.Diagnostics;

namespace Apotheosis.Components.Audio.Interfaces;

public interface IAudioStreamService
{ 
    Process? CreateStreamProcessAsync();
}