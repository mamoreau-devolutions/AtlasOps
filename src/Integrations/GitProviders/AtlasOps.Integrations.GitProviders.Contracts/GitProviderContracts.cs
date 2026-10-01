namespace AtlasOps.Integrations.GitProviders.Contracts;

public sealed record RepositoryReference(
    string ProviderId,
    string Owner,
    string Name,
    string DefaultBranch,
    bool Archived);

public sealed record PullRequestReference(
    string ProviderId,
    string Repository,
    long Number,
    string SourceBranch,
    string TargetBranch,
    string Revision,
    bool Draft,
    IReadOnlyList<string> Labels);

public sealed record BranchPolicy(
    int MinimumApprovals,
    bool RequirePassingChecks,
    bool RequireLinearHistory,
    bool AllowForcePush,
    IReadOnlySet<string> RequiredChecks);

public sealed record BranchPolicyEvaluation(
    bool Allowed,
    IReadOnlyList<string> Diagnostics);

public sealed record GitSynchronizationDelta(
    IReadOnlyList<PullRequestReference> Added,
    IReadOnlyList<PullRequestReference> Updated,
    IReadOnlyList<long> Removed,
    string NextCursor);
