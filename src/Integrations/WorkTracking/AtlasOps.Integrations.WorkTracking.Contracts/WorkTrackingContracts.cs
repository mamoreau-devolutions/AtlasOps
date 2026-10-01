namespace AtlasOps.Integrations.WorkTracking.Contracts;

public sealed record WorkItemReference(
    string ProviderId,
    string Project,
    string Key,
    string Title,
    string State,
    string Revision,
    int Priority,
    IReadOnlyList<string> Labels);

public sealed record WorkTransition(
    string Name,
    IReadOnlySet<string> FromStates,
    string TargetState,
    IReadOnlySet<string> RequiredFields);

public sealed record WorkItemMutation(
    string Key,
    string ExpectedRevision,
    IReadOnlyDictionary<string, string> Values);

public sealed record WorkTransitionResult(
    bool Succeeded,
    WorkItemReference Item,
    IReadOnlyList<string> Diagnostics);

public sealed record WorkSynchronizationDelta(
    IReadOnlyList<WorkItemReference> Changed,
    IReadOnlyList<string> DeletedKeys,
    string Watermark);
