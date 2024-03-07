namespace Apotheosis.Core.Features.Converse.Configuration;

public sealed class ConverseSettings
{
    public const string Name = "Converse";

    /// <summary>
    /// Gets or sets the voice system instructions for converse commands.
    /// </summary>
    public Dictionary<string, string>? VoiceSystemInstructions { get; set; }
}