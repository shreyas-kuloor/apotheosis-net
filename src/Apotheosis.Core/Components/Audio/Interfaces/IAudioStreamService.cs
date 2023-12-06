using System.Diagnostics;

namespace Apotheosis.Core.Components.Audio.Interfaces;

public interface IAudioStreamService
{ 
    Process? CreateStreamProcessAsync();
}