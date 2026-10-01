namespace AtlasOps.Features.Api.ApiGatewayProvisioning;

public sealed record ApiGatewayProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);