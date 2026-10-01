namespace AtlasOps.Features.Edge.EdgeGatewayProvisioning;

public sealed record EdgeGatewayProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);