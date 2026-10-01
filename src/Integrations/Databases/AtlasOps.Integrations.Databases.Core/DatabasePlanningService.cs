namespace AtlasOps.Integrations.Databases.Core;

using System.Text.RegularExpressions;

using AtlasOps.Integrations.Databases.Contracts;

public sealed partial class DatabasePlanningService
{
    public DatabaseQueryValidation Validate(
        DatabaseEndpoint endpoint,
        DatabaseQueryPlan plan)
    {
        List<string> diagnostics = [];

        if (endpoint.Provider != DatabaseProviderKind.Sqlite &&
            (string.IsNullOrWhiteSpace(endpoint.Host) || endpoint.Port is < 1 or > 65_535))
        {
            diagnostics.Add("Network database providers require a valid host and port.");
        }

        if (string.IsNullOrWhiteSpace(endpoint.Database))
        {
            diagnostics.Add("Database name is required.");
        }

        if (!endpoint.RequireTls && endpoint.Provider != DatabaseProviderKind.Sqlite)
        {
            diagnostics.Add("Remote database connections must require TLS.");
        }

        if (string.IsNullOrWhiteSpace(plan.CommandText))
        {
            diagnostics.Add("Command text is required.");
        }

        if (plan.Timeout < TimeSpan.FromMilliseconds(100) ||
            plan.Timeout > TimeSpan.FromHours(1))
        {
            diagnostics.Add("Command timeout must be between 100 milliseconds and one hour.");
        }

        if (plan.MaximumRows is < 1 or > 1_000_000)
        {
            diagnostics.Add("Maximum rows must be between 1 and 1,000,000.");
        }

        HashSet<string> declaredParameters = plan.Parameters
            .Select(static parameter => NormalizeName(parameter.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        string[] referencedParameters = ParameterName()
            .Matches(plan.CommandText)
            .Select(static match => NormalizeName(match.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (string parameter in referencedParameters)
        {
            if (!declaredParameters.Contains(parameter))
            {
                diagnostics.Add($"Parameter '{parameter}' is referenced but not declared.");
            }
        }

        if (plan.ReadOnly && MutationStatement().IsMatch(plan.CommandText))
        {
            diagnostics.Add("Read-only query plans cannot contain mutation statements.");
        }

        return new DatabaseQueryValidation(diagnostics.Count == 0, diagnostics);
    }

    public IReadOnlyDictionary<string, object?> CreateParameterMap(DatabaseQueryPlan plan)
    {
        Dictionary<string, object?> parameters = new(StringComparer.OrdinalIgnoreCase);
        foreach (DatabaseParameter parameter in plan.Parameters)
        {
            parameters[NormalizeName(parameter.Name)] = parameter.Value;
        }

        return parameters;
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().TrimStart('@', ':');
    }

    [GeneratedRegex("[@:][A-Za-z_][A-Za-z0-9_]*")]
    private static partial Regex ParameterName();

    [GeneratedRegex("\\b(?:INSERT|UPDATE|DELETE|MERGE|DROP|ALTER|CREATE|TRUNCATE)\\b", RegexOptions.IgnoreCase)]
    private static partial Regex MutationStatement();
}
