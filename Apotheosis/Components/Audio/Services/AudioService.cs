using System.Diagnostics;
using Apotheosis.Components.Audio.Interfaces;

namespace Apotheosis.Components.Audio.Services;

public class AudioService : IAudioService
{
    public async  Task<Stream> ConvertMp3ToPcmAsync(Stream mp3Stream)
    {
        var pcmStream = new MemoryStream();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = "-i pipe:0 -f s16le -ar 48000 -ac 2 pipe:1",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        using (var process = Process.Start(processStartInfo))
        {
            // Copy MP3 stream to standard input of FFmpeg
            await mp3Stream.CopyToAsync(process!.StandardInput.BaseStream);
            process.StandardInput.Close();

            // Read PCM stream from standard output of FFmpeg
            await process.StandardOutput.BaseStream.CopyToAsync(pcmStream);

            await process.WaitForExitAsync();
        }

        pcmStream.Position = 0; // Reset the position of the PCM stream
        return pcmStream;
    }

    public async Task<Process> CreateStreamProcessAsync(Stream pcmStream)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
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