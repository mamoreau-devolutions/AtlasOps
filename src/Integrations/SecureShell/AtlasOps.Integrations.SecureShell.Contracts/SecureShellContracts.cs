namespace AtlasOps.Integrations.SecureShell.Contracts;

public sealed record SecureShellEndpoint(
    string Host,
    int Port,
    string UserName,
    string CredentialReference,
    TimeSpan Timeout);

public sealed record HostKeyRecord(
    string Host,
    int Port,
    string Algorithm,
    string Fingerprint,
    DateTimeOffset AcceptedAt,
    DateTimeOffset? ExpiresAt);

public sealed record SecureCommandPlan(
    string Executable,
    IReadOnlyList<string> Arguments,
    IReadOnlyDictionary<string, string> Environment,
    string WorkingDirectory,
    bool AllocateTerminal);

public sealed record SecureCommandValidation(
    bool Valid,
    IReadOnlyList<string> Diagnostics);
