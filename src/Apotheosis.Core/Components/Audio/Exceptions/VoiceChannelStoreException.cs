namespace Apotheosis.Core.Components.Audio.Exceptions;

public sealed class VoiceChannelStoreException : Exception
{
    public VoiceChannelStoreException()
    {
    }

    public VoiceChannelStoreException(string message) : base(message)
    {
    }

    public VoiceChannelStoreException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}