﻿namespace Apotheosis.Core.Components.TextToSpeech.Exceptions;

public sealed class TextToSpeechNetworkException : Exception
{
    public TextToSpeechNetworkException()
    {
    }

    public TextToSpeechNetworkException(string message) : base(message)
    {
    }

    public TextToSpeechNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}