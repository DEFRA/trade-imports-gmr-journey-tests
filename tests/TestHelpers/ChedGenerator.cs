using System.Security.Cryptography;

namespace TestHelpers;

public static class ChedGenerator
{
    private const string ChedCharacters = "0123456789";

    public enum ChedType
    {
        CHEDA,
        CHEDD,
        CHEDP,
        CHEDPP,
    }

    public static string GenerateChed(ChedType chedType = ChedType.CHEDPP)
    {
        var randomCharacters = new string(
            Enumerable
                .Range(0, 7)
                .Select(_ => ChedCharacters[RandomNumberGenerator.GetInt32(ChedCharacters.Length)])
                .ToArray()
        );

        return $"{chedType}.GB.{DateTime.UtcNow:yyyy}GB{randomCharacters}";
    }
}
