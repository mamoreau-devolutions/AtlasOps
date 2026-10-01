namespace AtlasOps.Features.Governance.PolicySimulation;

public sealed record UpdatePolicySimulationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);