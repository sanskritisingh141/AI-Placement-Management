using AIPlacement.Application.Authentication.Services;

namespace AIPlacement.Tests.Authentication;

public class PasswordHashServiceTests
{
    [Fact]
    public void HashUsesSaltAndVerifiesCorrectPassword()
    {
        var service = new Pbkdf2PasswordHashService();
        var first = service.Hash("Sufficiently-Strong-Password");
        var second = service.Hash("Sufficiently-Strong-Password");
        Assert.NotEqual(first, second);
        Assert.True(service.Verify("Sufficiently-Strong-Password", first));
        Assert.False(service.Verify("wrong-password", first));
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("pbkdf2$bad$data")]
    public void VerifyRejectsMalformedHashes(string value)
    {
        var service = new Pbkdf2PasswordHashService();
        Assert.False(service.Verify("password", value));
    }
}
