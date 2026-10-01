namespace AtlasOps.Features.Architecture.ArchitectureStandardProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureStandardProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureStandardProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureStandardProvisioningValidator validator = new();
    private readonly ArchitectureStandardProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureStandardProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureStandardProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureStandardProvisioningChanged>.Invalid(issues);
        }

        ArchitectureStandardProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureStandardProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureStandardProvisioningChanged>.Invalid(
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

        ArchitectureStandardProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureStandardProvisioningChanged>.Success(changed);
    }
}