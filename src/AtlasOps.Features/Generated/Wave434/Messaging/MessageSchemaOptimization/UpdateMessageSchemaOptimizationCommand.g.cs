namespace AtlasOps.Features.Messaging.MessageSchemaOptimization;

public sealed record UpdateMessageSchemaOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);