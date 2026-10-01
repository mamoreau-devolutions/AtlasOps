namespace AtlasOps.Features.Connections.DatabaseConnection;

public sealed record UpdateDatabaseConnectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);