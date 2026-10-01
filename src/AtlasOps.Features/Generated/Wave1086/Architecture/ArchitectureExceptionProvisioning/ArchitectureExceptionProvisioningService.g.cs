namespace AtlasOps.Features.Architecture.ArchitectureExceptionProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureExceptionProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureExceptionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureExceptionProvisioningValidator validator = new();
    private readonly ArchitectureExceptionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureExceptionProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureExceptionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureExceptionProvisioningChanged>.Invalid(issues);
        }

        ArchitectureExceptionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureExceptionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureExceptionProvisioningChanged>.Invalid(
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

        ArchitectureExceptionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureExceptionProvisioningChanged>.Success(changed);
    }
}