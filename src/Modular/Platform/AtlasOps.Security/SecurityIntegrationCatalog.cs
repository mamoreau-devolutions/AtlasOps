namespace AtlasOps.Security;

using System.IdentityModel.Tokens.Jwt;

using Org.BouncyCastle.Crypto.Digests;

using PgpCore;
using Renci.SshNet;
using Sodium;
using VaultSharp;
using Yubico.YubiKey;

public sealed record SecurityIntegrationDescriptor(string Id, Type IntegrationType, bool HandlesSecrets, bool RequiresHardware);

public static class SecurityIntegrationCatalog
{
    public static IReadOnlyList<SecurityIntegrationDescriptor> Integrations { get; } =
    [
        new("jwt", typeof(JwtSecurityTokenHandler), true, false),
        new("bouncy-castle", typeof(Sha256Digest), false, false),
        new("openpgp", typeof(PGP), true, false),
        new("sodium", typeof(SecretBox), true, false),
        new("ssh", typeof(SshClient), true, false),
        new("vault", typeof(VaultClient), true, false),
        new("yubikey", typeof(YubiKeyDevice), true, true),
    ];
}

public sealed record JwtInspectionResult(bool Valid, string Algorithm, string? Subject, DateTimeOffset? ExpiresAt, IReadOnlyList<string> Diagnostics);

public sealed class JwtInspector
{
    private static readonly IReadOnlySet<string> AllowedAlgorithms = new HashSet<string>(
        ["RS256", "RS384", "RS512", "ES256", "ES384", "ES512", "PS256", "PS384", "PS512"],
        StringComparer.Ordinal);

    public JwtInspectionResult Inspect(string encodedToken, DateTimeOffset now)
    {
        JwtSecurityTokenHandler handler = new();
        if (!handler.CanReadToken(encodedToken))
        {
            return new(false, string.Empty, null, null, ["Token format is invalid."]);
        }

        JwtSecurityToken token = handler.ReadJwtToken(encodedToken);
        List<string> diagnostics = [];
        if (!AllowedAlgorithms.Contains(token.Header.Alg))
        {
            diagnostics.Add($"Algorithm '{token.Header.Alg}' is not permitted.");
        }

        DateTimeOffset? expiresAt = token.Payload.Exp is int expiration
            ? DateTimeOffset.FromUnixTimeSeconds(expiration)
            : null;
        if (expiresAt is null)
        {
            diagnostics.Add("Token does not contain an expiration.");
        }
        else if (expiresAt <= now)
        {
            diagnostics.Add("Token has expired.");
        }

        return new(
            diagnostics.Count == 0,
            token.Header.Alg,
            token.Subject,
            expiresAt,
            diagnostics);
    }
}
