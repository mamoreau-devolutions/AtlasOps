namespace AtlasOps.Features.Api.ApiContractProvisioning;

using AtlasOps.Features;

public sealed class ApiContractProvisioningService(
    IAtlasOpsCapabilityRepository<ApiContractProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiContractProvisioningValidator validator = new();
    private readonly ApiContractProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiContractProvisioningChanged>> ExecuteAsync(
        UpdateApiContractProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiContractProvisioningChanged>.Invalid(issues);
        }

        ApiContractProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiContractProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiContractProvisioningChanged>.Invalid(
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

        ApiContractProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiContractProvisioningChanged>.Success(changed);
    }
}