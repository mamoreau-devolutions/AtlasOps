namespace AtlasOps.Features.Automation.ManualGate;

public sealed record UpdateManualGateCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);