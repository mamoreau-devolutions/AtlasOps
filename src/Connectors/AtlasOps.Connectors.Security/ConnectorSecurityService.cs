namespace AtlasOps.Connectors.Security;

using System.DirectoryServices;
using System.Security.Cryptography;

using Fido2NetLib;

using Konscious.Security.Cryptography;

using OtpSharp;

using PemUtils;

public sealed record SecurityPackageDescriptor(string Id, Type PrimaryType, string Capability);

public sealed record SecretDerivationOptions(int Iterations, int MemorySizeKb, int DegreeOfParallelism, int OutputLength);

public sealed record SecretDerivationResult(byte[] DerivedKey, string Algorithm, SecretDerivationOptions Options);

public static class ConnectorSecurityCatalog
{
    public static IReadOnlyList<SecurityPackageDescriptor> Packages { get; } =
    [
        new("argon2", typeof(Argon2id), "Memory-hard secret derivation"),
        new("blake2", typeof(HMACBlake2B), "Keyed Blake2 hashing"),
        new("fido2", typeof(Fido2), "Passkey and hardware authentication"),
        new("otp", typeof(Totp), "Time-based one-time passwords"),
        new("pem", typeof(PemReader), "PEM key material parsing"),
        new("directory-services", typeof(DirectoryEntry), "Directory identity integration"),
    ];
}

public sealed class ConnectorSecretDeriver
{
    public async ValueTask<SecretDerivationResult> DeriveAsync(
        ReadOnlyMemory<byte> secret,
        ReadOnlyMemory<byte> salt,
        SecretDerivationOptions options,
        CancellationToken cancellationToken)
    {
        if (secret.IsEmpty)
        {
            throw new ArgumentException("Secret material cannot be empty.", nameof(secret));
        }

        if (salt.Length < 16)
        {
            throw new ArgumentException("Salt must contain at least 16 bytes.", nameof(salt));
        }

        if (options.Iterations is < 2 or > 20 ||
            options.MemorySizeKb is < 8_192 or > 1_048_576 ||
            options.DegreeOfParallelism is < 1 or > 64 ||
            options.OutputLength is < 16 or > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Argon2 parameters are outside the supported security bounds.");
        }

        using Argon2id argon = new(secret.ToArray())
        {
            Salt = salt.ToArray(),
            Iterations = options.Iterations,
            MemorySize = options.MemorySizeKb,
            DegreeOfParallelism = options.DegreeOfParallelism,
        };

        byte[] derived = await argon.GetBytesAsync(options.OutputLength).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return new SecretDerivationResult(derived, "Argon2id", options);
    }

    public static byte[] CreateSalt(int length = 32)
    {
        if (length is < 16 or > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        return RandomNumberGenerator.GetBytes(length);
    }
}
