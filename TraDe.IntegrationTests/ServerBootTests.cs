using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TraDe.IntegrationTests;

public class ServerBootTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ServerBootTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Server_Should_Start_Without_Crashing()
    {
        // Act
        var client = _factory.CreateClient();

        // Assert
        // If CreateClient() succeeds, the DI container and pipeline are valid.
        Assert.NotNull(client);
    }
}