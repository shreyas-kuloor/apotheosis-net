﻿using Newtonsoft.Json;

namespace Apotheosis.Core.Features.AiChat.Models;

public sealed class AiChatMessage
{
    /// <summary>
    /// Gets or sets the role of the AI chat message.
    /// </summary>
    [JsonProperty("role")]
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the content of the AI chat message.
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; set; }
}