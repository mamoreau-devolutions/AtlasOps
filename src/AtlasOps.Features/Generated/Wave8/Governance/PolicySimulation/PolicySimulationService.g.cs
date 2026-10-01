namespace AtlasOps.Features.Governance.PolicySimulation;

using AtlasOps.Features;

public sealed class PolicySimulationService(
    IAtlasOpsCapabilityRepository<PolicySimulationItem> repository,
    TimeProvider timeProvider)
{
    private readonly PolicySimulationValidator validator = new();
    private readonly PolicySimulationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PolicySimulationChanged>> ExecuteAsync(
        UpdatePolicySimulationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PolicySimulationChanged>.Invalid(issues);
        }

        PolicySimulationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PolicySimulationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PolicySimulationChanged>.Invalid(
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

        PolicySimulationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PolicySimulationChanged>.Success(changed);
    }
}