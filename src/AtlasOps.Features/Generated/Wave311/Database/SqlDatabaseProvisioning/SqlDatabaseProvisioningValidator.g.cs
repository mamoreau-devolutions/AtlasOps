namespace AtlasOps.Features.Database.SqlDatabaseProvisioning;

using AtlasOps.Features;

public sealed class SqlDatabaseProvisioningValidator
{
    public IReadOnlyList<AtlasOpsValidationIssue> Validate(UpdateSqlDatabaseProvisioningCommand command)
    {
        List<AtlasOpsValidationIssue> issues = [];

        if (string.IsNullOrWhiteSpace(command.Id))
        {
            issues.Add(new("Id", "An identifier is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            issues.Add(new("Name", "A name is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Owner))
        {
            issues.Add(new("Owner", "An owner is required."));
        }

        if (string.IsNullOrWhiteSpace(command.TargetState))
        {
            issues.Add(new("State", "A target state is required."));
        }

        if (command.Priority is < 1 or > 10)
        {
            issues.Add(new("Priority", "Priority must be between 1 and 10."));
        }

        return issues;
    }
}