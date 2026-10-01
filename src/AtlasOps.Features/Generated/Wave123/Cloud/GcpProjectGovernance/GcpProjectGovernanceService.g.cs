namespace AtlasOps.Features.Cloud.GcpProjectGovernance;

using AtlasOps.Features;

public sealed class GcpProjectGovernanceService(
    IAtlasOpsCapabilityRepository<GcpProjectGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly GcpProjectGovernanceValidator validator = new();
    private readonly GcpProjectGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<GcpProjectGovernanceChanged>> ExecuteAsync(
        UpdateGcpProjectGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GcpProjectGovernanceChanged>.Invalid(issues);
        }

        GcpProjectGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GcpProjectGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GcpProjectGovernanceChanged>.Invalid(
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

        GcpProjectGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GcpProjectGovernanceChanged>.Success(changed);
    }
}