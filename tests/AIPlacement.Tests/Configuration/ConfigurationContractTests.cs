using System.Text.Json;

namespace AIPlacement.Tests.Configuration;

public class ConfigurationContractTests
{
    [Fact]
    public void ApiDevelopmentSettingsProvideTheJwtKeysUsedByTheApplication()
    {
        var root = FindRepositoryRoot();
        var path = Path.Combine(root, "src", "AIPlacement.API", "appsettings.Development.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var jwt = document.RootElement.GetProperty("Jwt");

        Assert.True(jwt.TryGetProperty("Key", out var key));
        Assert.True(key.GetString()!.Length >= 32);
        Assert.False(string.IsNullOrWhiteSpace(jwt.GetProperty("Issuer").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(jwt.GetProperty("Audience").GetString()));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AIPlacement.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
