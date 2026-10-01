namespace AtlasOps.Integrations.Identity.Contracts;

public sealed record IdentitySubject(
    string ProviderId,
    string SubjectId,
    string UserName,
    bool Enabled,
    string Revision,
    IReadOnlyDictionary<string, string> Attributes);

public sealed record AccessGrant(
    string SubjectId,
    string Resource,
    string Role,
    DateTimeOffset GrantedAt,
    DateTimeOffset? ExpiresAt,
    string Source);

public enum IdentityChangeKind
{
    DisableSubject,
    RevokeGrant,
    UpdateSubject,
    AddGrant,
    EnableSubject,
}

public sealed record IdentityChange(
    IdentityChangeKind Kind,
    string SubjectId,
    string Resource,
    string? Role,
    int Order,
    string Reason);

public sealed record IdentityReconciliationPlan(
    IReadOnlyList<IdentityChange> Changes,
    IReadOnlyList<string> Diagnostics);
