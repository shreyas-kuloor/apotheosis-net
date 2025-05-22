namespace Apotheosis.Test.Unit.Components.Audio.Services;

public sealed class VoiceClientServiceTests
{
    readonly VoiceClientService _voiceClientService = new();

    [Fact]
    public void StoreVoiceClient_DoesNotThrowException_GivenSuccessfulAdd()
    {
        const ulong voiceChannelId = 1234;
        const ulong userId = 123;
        const string sessionId = "session123";
        const string endpoint = "endpoint";
        const ulong guildId = 321;
        const string token = "token";

        var voiceClient = new VoiceClient(userId, sessionId, endpoint, guildId, token);

        var exception = Record.Exception(() => _voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient));

        exception.Should().BeNull();

        var voiceClientExists = _voiceClientService.TryGetVoiceClient(voiceChannelId, out var existingVoiceClient);

        voiceClientExists.Should().BeTrue();
        existingVoiceClient.Should().BeSameAs(voiceClient);
    }

    [Fact]
    public void StoreVoiceClient_ThrowsVoiceChannelStoreException_GivenUnsuccessfulAdd()
    {
        const ulong voiceChannelId = 1234;
        const ulong userId = 123;
        const string sessionId = "session123";
        const string endpoint = "endpoint";
        const ulong guildId = 321;
        const string token = "token";

        var voiceClient = new VoiceClient(userId, sessionId, endpoint, guildId, token);

        _voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

        var actual = () => _voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

        actual.Should().Throw<VoiceChannelStoreException>()
            .Where(e => e.Message == "Voice channel already exists for guild ID.");
    }

    [Fact]
    public void TryGetVoiceClient_ReturnsTrueAndVoiceClient_GivenVoiceClientExists()
    {
        const ulong voiceChannelId = 1234;
        const ulong userId = 123;
        const string sessionId = "session123";
        const string endpoint = "endpoint";
        const ulong guildId = 321;
        const string token = "token";

        var voiceClient = new VoiceClient(userId, sessionId, endpoint, guildId, token);

        _voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

        var actual = _voiceClientService.TryGetVoiceClient(voiceChannelId, out var actualVoiceClient);

        actual.Should().BeTrue();
        actualVoiceClient.Should().BeSameAs(voiceClient);
    }

    [Fact]
    public void TryGetVoiceClient_ReturnsFalseAndNullVoiceClient_GivenVoiceClientDoesNotExist()
    {
        const ulong voiceChannelId = 1234;

        var actual = _voiceClientService.TryGetVoiceClient(voiceChannelId, out var actualVoiceClient);

        actual.Should().BeFalse();
        actualVoiceClient.Should().BeNull();
    }

    [Fact]
    public void TryRemoveVoiceClient_ReturnsTrueAndRemovesVoiceClient_GivenVoiceClientExists()
    {
        const ulong voiceChannelId = 1234;
        const ulong userId = 123;
        const string sessionId = "session123";
        const string endpoint = "endpoint";
        const ulong guildId = 321;
        const string token = "token";

        var voiceClient = new VoiceClient(userId, sessionId, endpoint, guildId, token);

        _voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

        var actual = _voiceClientService.TryRemoveVoiceClient(voiceChannelId, out var actualVoiceClient);

        actual.Should().BeTrue();
        actualVoiceClient.Should().BeSameAs(voiceClient);

        var voiceClientStillExists = _voiceClientService.TryGetVoiceClient(voiceChannelId, out var existingVoiceClient);

        voiceClientStillExists.Should().BeFalse();
        existingVoiceClient.Should().BeNull();
    }

    [Fact]
    public void TryRemoveVoiceClient_ReturnsFalseAndNullVoiceClient_GivenVoiceClientDoesNotExist()
    {
        const ulong voiceChannelId = 1234;

        var actual = _voiceClientService.TryRemoveVoiceClient(voiceChannelId, out var actualVoiceClient);

        actual.Should().BeFalse();
        actualVoiceClient.Should().BeNull();
    }
}