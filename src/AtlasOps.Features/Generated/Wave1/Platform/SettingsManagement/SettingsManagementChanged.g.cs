namespace AtlasOps.Features.Platform.SettingsManagement;

public sealed record SettingsManagementChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);