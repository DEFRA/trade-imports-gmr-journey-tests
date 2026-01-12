using System.Security.Cryptography;

namespace TestHelpers;

public static class MrnGenerator
{
    private const string MrnCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public enum MrnStatus
    {
        NoGmrs = 0,
        NotFinalisable = 1,
        CheckedIn = 2,
        Embarked = 3,
        Completed = 4,
        CheckedInInspectionRequired = 5,
        EmbarkedInspectionRequired = 6,
        NotFinalisableAndCheckedIn = 7,
    }

    public static string GenerateMrn(MrnStatus mrnStatus = MrnStatus.Embarked)
    {
        var randomCharacters = new string(
            Enumerable
                .Range(0, 13)
                .Select(_ => MrnCharacters[RandomNumberGenerator.GetInt32(MrnCharacters.Length)])
                .ToArray()
        );

        return $"{DateTime.UtcNow:yy}GB{randomCharacters}{(int)mrnStatus}";
    }
}
