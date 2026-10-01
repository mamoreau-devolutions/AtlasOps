namespace AtlasOps.Features.Compute.ComputeConsoleProvisioning;

using AtlasOps.Features;

public sealed class ComputeConsoleProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeConsoleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeConsoleProvisioningValidator validator = new();
    private readonly ComputeConsoleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeConsoleProvisioningChanged>> ExecuteAsync(
        UpdateComputeConsoleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeConsoleProvisioningChanged>.Invalid(issues);
        }

        ComputeConsoleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeConsoleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeConsoleProvisioningChanged>.Invalid(
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

        ComputeConsoleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeConsoleProvisioningChanged>.Success(changed);
    }
}