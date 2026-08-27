namespace AIPlacement.Application.Authentication.Interfaces;

public interface IPasswordHashService
{
    string Hash(string password);
    bool Verify(string password, string encodedHash);
}
