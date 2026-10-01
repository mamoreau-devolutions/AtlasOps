namespace AtlasOps.Features.Platform.DialogCoordination;

public sealed record UpdateDialogCoordinationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);