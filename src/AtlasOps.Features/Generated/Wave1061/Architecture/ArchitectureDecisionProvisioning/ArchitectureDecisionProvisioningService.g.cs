namespace AtlasOps.Features.Architecture.ArchitectureDecisionProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureDecisionProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureDecisionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDecisionProvisioningValidator validator = new();
    private readonly ArchitectureDecisionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDecisionProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureDecisionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDecisionProvisioningChanged>.Invalid(issues);
        }

        ArchitectureDecisionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDecisionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDecisionProvisioningChanged>.Invalid(
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

        ArchitectureDecisionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDecisionProvisioningChanged>.Success(changed);
    }
}