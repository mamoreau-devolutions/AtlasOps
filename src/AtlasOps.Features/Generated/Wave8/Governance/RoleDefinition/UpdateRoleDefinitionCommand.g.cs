namespace AtlasOps.Features.Governance.RoleDefinition;

public sealed record UpdateRoleDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);