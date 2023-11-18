﻿namespace Apotheosis.Components.GCPDot.Exceptions;

public class GcpDotNetworkException : Exception
{
    public GcpDotNetworkException()
    {
    }

    public GcpDotNetworkException(string message) : base(message)
    {
    }

    public GcpDotNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}