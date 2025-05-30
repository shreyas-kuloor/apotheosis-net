﻿namespace Apotheosis.Core.Features.AiChat.Interfaces;

public interface IAiChatService
{
    Task<string> GetChatResponseAsync(string prompt, CancellationToken cancellationToken);
}