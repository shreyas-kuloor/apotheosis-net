using System.Diagnostics;
using Apotheosis.Components.Audio.Interfaces;

namespace Apotheosis.Components.Audio.Services;

public class AudioService : IAudioService
{ 
    public async Task<Process> CreateStreamProcessAsync(Stream pcmStream)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = "-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            }
        };

        process.Start();
        
        await pcmStream.CopyToAsync(process.StandardInput.BaseStream);
        
        process.StandardInput.Close();

        return process;
    }
}