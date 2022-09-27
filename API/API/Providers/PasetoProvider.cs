using API.Database;
using Paseto;
using Paseto.Builder;
using Paseto.Cryptography.Key;

namespace API.Providers;

public class PasetoProvider
{
    public string GenerateAccessToken(User userModel, PasetoSymmetricKey pasetoSymmetricKey)
    {
        var token = new PasetoBuilder().Use(ProtocolVersion.V4, Purpose.Local)
            .WithKey(pasetoSymmetricKey)
            .AddClaim("id", userModel.Id.ToString())
            .AddClaim("username", userModel.UserName)
            .Issuer("https://github.com/daviddesmet/paseto-dotnet")
            .Subject(Guid.NewGuid().ToString())
            .Audience("https://paseto.io")
            .NotBefore(DateTime.UtcNow.AddMinutes(5))
            .IssuedAt(DateTime.UtcNow)
            .Expiration(DateTime.UtcNow.AddHours(1))
            .TokenIdentifier("123456ABCD")
            .AddFooter("authEstantaToken")
            .Encode();
        return token;
    }
}