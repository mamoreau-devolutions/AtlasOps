namespace AtlasOps.Features.Api.ApiQuotaProvisioning;

using AtlasOps.Features;

public sealed class ApiQuotaProvisioningService(
    IAtlasOpsCapabilityRepository<ApiQuotaProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiQuotaProvisioningValidator validator = new();
    private readonly ApiQuotaProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiQuotaProvisioningChanged>> ExecuteAsync(
        UpdateApiQuotaProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiQuotaProvisioningChanged>.Invalid(issues);
        }

        ApiQuotaProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiQuotaProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiQuotaProvisioningChanged>.Invalid(
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

        ApiQuotaProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiQuotaProvisioningChanged>.Success(changed);
    }
}