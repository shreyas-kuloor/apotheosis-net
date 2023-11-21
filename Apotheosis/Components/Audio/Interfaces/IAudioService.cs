using System.Diagnostics;

namespace Apotheosis.Components.Audio.Interfaces;

public interface IAudioService
{ 
    Task<Process> CreateStreamProcessAsync(Stream pcmStream);
}