namespace AtlasOps.Features.ServiceManagement.ProblemRecordProvisioning;

using AtlasOps.Features;

public sealed class ProblemRecordProvisioningService(
    IAtlasOpsCapabilityRepository<ProblemRecordProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProblemRecordProvisioningValidator validator = new();
    private readonly ProblemRecordProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProblemRecordProvisioningChanged>> ExecuteAsync(
        UpdateProblemRecordProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProblemRecordProvisioningChanged>.Invalid(issues);
        }

        ProblemRecordProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProblemRecordProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProblemRecordProvisioningChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        ProblemRecordProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProblemRecordProvisioningChanged>.Success(changed);
    }
}