namespace AtlasOps.Features.Compute.ComputeTemplateProvisioning;

using AtlasOps.Features;

public sealed class ComputeTemplateProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeTemplateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeTemplateProvisioningValidator validator = new();
    private readonly ComputeTemplateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeTemplateProvisioningChanged>> ExecuteAsync(
        UpdateComputeTemplateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeTemplateProvisioningChanged>.Invalid(issues);
        }

        ComputeTemplateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeTemplateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeTemplateProvisioningChanged>.Invalid(
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

        ComputeTemplateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeTemplateProvisioningChanged>.Success(changed);
    }
}