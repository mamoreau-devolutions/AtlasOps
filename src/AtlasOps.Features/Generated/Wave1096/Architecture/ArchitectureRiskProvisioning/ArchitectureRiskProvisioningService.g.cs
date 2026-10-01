namespace AtlasOps.Features.Architecture.ArchitectureRiskProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureRiskProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureRiskProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRiskProvisioningValidator validator = new();
    private readonly ArchitectureRiskProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRiskProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureRiskProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRiskProvisioningChanged>.Invalid(issues);
        }

        ArchitectureRiskProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRiskProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRiskProvisioningChanged>.Invalid(
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

        ArchitectureRiskProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRiskProvisioningChanged>.Success(changed);
    }
}