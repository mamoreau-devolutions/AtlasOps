namespace AtlasOps.Features.Inventory.CertificateInventory;

using AtlasOps.Features;

public sealed class CertificateInventoryService(
    IAtlasOpsCapabilityRepository<CertificateInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CertificateInventoryValidator validator = new();
    private readonly CertificateInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CertificateInventoryChanged>> ExecuteAsync(
        UpdateCertificateInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CertificateInventoryChanged>.Invalid(issues);
        }

        CertificateInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CertificateInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CertificateInventoryChanged>.Invalid(
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

        CertificateInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CertificateInventoryChanged>.Success(changed);
    }
}