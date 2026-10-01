namespace AtlasOps.Features.Platform.SettingsManagement;

public sealed record UpdateSettingsManagementCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);