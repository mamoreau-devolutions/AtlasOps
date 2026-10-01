namespace AtlasOps.Integrations.Databases.Contracts;

public enum DatabaseProviderKind
{
    Sqlite,
    SqlServer,
    PostgreSql,
    MySql,
    Oracle,
}

public sealed record DatabaseEndpoint(
    DatabaseProviderKind Provider,
    string Host,
    int Port,
    string Database,
    string CredentialReference,
    bool RequireTls);

public sealed record DatabaseParameter(
    string Name,
    object? Value,
    string TypeName,
    bool Sensitive);

public sealed record DatabaseQueryPlan(
    string CommandText,
    IReadOnlyList<DatabaseParameter> Parameters,
    TimeSpan Timeout,
    bool ReadOnly,
    int MaximumRows);

public sealed record DatabaseQueryValidation(
    bool Valid,
    IReadOnlyList<string> Diagnostics);
