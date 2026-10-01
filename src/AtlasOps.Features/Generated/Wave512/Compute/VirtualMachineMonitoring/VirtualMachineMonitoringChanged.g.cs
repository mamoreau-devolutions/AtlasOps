namespace AtlasOps.Features.Compute.VirtualMachineMonitoring;

public sealed record VirtualMachineMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);