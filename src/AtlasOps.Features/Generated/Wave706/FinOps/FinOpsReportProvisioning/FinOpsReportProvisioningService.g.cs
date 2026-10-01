namespace AtlasOps.Features.FinOps.FinOpsReportProvisioning;

using AtlasOps.Features;

public sealed class FinOpsReportProvisioningService(
    IAtlasOpsCapabilityRepository<FinOpsReportProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly FinOpsReportProvisioningValidator validator = new();
    private readonly FinOpsReportProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FinOpsReportProvisioningChanged>> ExecuteAsync(
        UpdateFinOpsReportProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FinOpsReportProvisioningChanged>.Invalid(issues);
        }

        FinOpsReportProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FinOpsReportProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FinOpsReportProvisioningChanged>.Invalid(
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

        FinOpsReportProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FinOpsReportProvisioningChanged>.Success(changed);
    }
}