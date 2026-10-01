namespace AtlasOps.Features.Platform.ModuleRegistration;

public sealed record ModuleRegistrationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);