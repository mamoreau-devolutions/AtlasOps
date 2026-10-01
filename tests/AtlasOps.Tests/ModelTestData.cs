namespace AtlasOps.Tests;

using System.Reflection;

using AtlasOps.Core;
using AtlasOps.Core.Generated;

public sealed record ModelCase(Type Type, string Category, string DisplayName);

internal static class ModelTestData
{
    internal static readonly DateTimeOffset FixedTimestamp =
        new(2025, 1, 2, 3, 4, 5, TimeSpan.Zero);

    internal static IReadOnlyList<ModelCase> Cases { get; } =
    [
        new(typeof(AtlasOpsActivity), "Organization", "Activities"),
        new(typeof(AtlasOpsAlert), "Operations", "Alerts"),
        new(typeof(AtlasOpsAuditEvent), "Governance", "Audit events"),
        new(typeof(AtlasOpsConnection), "Connections", "Connections"),
        new(typeof(AtlasOpsCredential), "Connections", "Credentials"),
        new(typeof(AtlasOpsDashboard), "Observability", "Dashboards"),
        new(typeof(AtlasOpsDashboardCard), "Observability", "Dashboard cards"),
        new(typeof(AtlasOpsDeployment), "Operations", "Deployments"),
        new(typeof(AtlasOpsEditorDocument), "Editor", "Documents"),
        new(typeof(AtlasOpsEnvironment), "Workspace", "Environments"),
        new(typeof(AtlasOpsHost), "Infrastructure", "Hosts"),
        new(typeof(AtlasOpsIncident), "Operations", "Incidents"),
        new(typeof(AtlasOpsMilestone), "Planning", "Milestones"),
        new(typeof(AtlasOpsPolicy), "Governance", "Policies"),
        new(typeof(AtlasOpsProject), "Workspace", "Projects"),
        new(typeof(AtlasOpsQuery), "Editor", "Queries"),
        new(typeof(AtlasOpsRunbook), "Operations", "Runbooks"),
        new(typeof(AtlasOpsService), "Infrastructure", "Services"),
        new(typeof(AtlasOpsTaskItem), "Planning", "Tasks"),
        new(typeof(AtlasOpsTeam), "Organization", "Teams"),
    ];

    public static IEnumerable<object[]> GetCases()
    {
        return Cases.Select(static modelCase => new object[] { modelCase });
    }

    internal static IAtlasOpsEntity CreatePopulated(ModelCase modelCase)
    {
        IAtlasOpsEntity entity = AtlasOpsGeneratedWorkspaceManager.Create(modelCase.Type.Name);
        entity.Id = $"id-{modelCase.Type.Name}";
        entity.UpdatedAt = FixedTimestamp;

        foreach (PropertyInfo property in EditableProperties(modelCase.Type))
        {
            property.SetValue(entity, SampleValue(property));
        }

        return entity;
    }

    internal static IReadOnlyList<PropertyInfo> EditableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(static property =>
                property.SetMethod is not null &&
                property.Name is not nameof(IAtlasOpsEntity.Id) and not nameof(IAtlasOpsEntity.UpdatedAt))
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .ToArray();
    }

    internal static void AssertAllPropertiesEqual(IAtlasOpsEntity expected, IAtlasOpsEntity actual)
    {
        Assert.AreEqual(expected.GetType(), actual.GetType());
        foreach (PropertyInfo property in expected.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            Assert.AreEqual(
                property.GetValue(expected),
                property.GetValue(actual),
                $"{expected.GetType().Name}.{property.Name} did not round-trip.");
        }
    }

    private static object SampleValue(PropertyInfo property)
    {
        return property.PropertyType == typeof(string)
            ? $"sample-{property.Name}-\"quoted\"\\path{Environment.NewLine}Ω"
            : property.PropertyType == typeof(bool)
                ? true
                : property.PropertyType == typeof(int)
                    ? 42
                    : property.PropertyType == typeof(long)
                        ? 42_000_000_000L
                        : property.PropertyType == typeof(double)
                            ? 12.5D
                            : property.PropertyType == typeof(decimal)
                                ? 12.5M
                                : property.PropertyType == typeof(DateTimeOffset)
                                    ? FixedTimestamp.AddDays(1)
                                    : throw new InvalidOperationException(
                                        $"No sample value exists for {property.PropertyType.FullName}.");
    }
}

internal sealed class UnsupportedEntity : IAtlasOpsEntity
{
    public string Id { get; set; } = "unsupported";

    public DateTimeOffset UpdatedAt { get; set; } = ModelTestData.FixedTimestamp;
}