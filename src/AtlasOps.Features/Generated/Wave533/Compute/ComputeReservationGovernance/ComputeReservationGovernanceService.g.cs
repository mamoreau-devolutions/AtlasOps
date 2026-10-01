namespace AtlasOps.Features.Compute.ComputeReservationGovernance;

using AtlasOps.Features;

public sealed class ComputeReservationGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeReservationGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeReservationGovernanceValidator validator = new();
    private readonly ComputeReservationGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeReservationGovernanceChanged>> ExecuteAsync(
        UpdateComputeReservationGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeReservationGovernanceChanged>.Invalid(issues);
        }

        ComputeReservationGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeReservationGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeReservationGovernanceChanged>.Invalid(
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

        ComputeReservationGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeReservationGovernanceChanged>.Success(changed);
    }
}