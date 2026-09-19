namespace ZALaw.Api.Services;

/// <summary>Generates application numbers in the ZA-YYYY-NNNNN format used across the portal.</summary>
public static class ApplicationNumberGenerator
{
    public static string Generate(int taxYear, int sequence) =>
        $"ZA-{taxYear}-{sequence.ToString().PadLeft(5, '0')}";
}
