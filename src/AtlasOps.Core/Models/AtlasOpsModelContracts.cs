namespace AtlasOps.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateAtlasOpsModelAttribute : Attribute
{
    public GenerateAtlasOpsModelAttribute(string category, string displayName)
    {
        this.Category = category;
        this.DisplayName = displayName;
    }

    public string Category { get; }

    public string DisplayName { get; }
}

public interface IAtlasOpsEntity
{
    string Id { get; set; }

    DateTimeOffset UpdatedAt { get; set; }
}

public sealed class AtlasOpsSettings
{
    public string WorkspaceName { get; set; } = "AtlasOps Workspace";

    public string Theme { get; set; } = "Dark";

    public string TursoUrl { get; set; } = string.Empty;

    public string TursoToken { get; set; } = string.Empty;

    public bool UseTurso { get; set; }

    public string LocalDataPath { get; set; } = string.Empty;
}

public sealed class AtlasOpsLayoutState
{
    public string SelectedRegion { get; set; } = "Workspace";

    public string SelectedModelType { get; set; } = string.Empty;

    public double NavigationWidth { get; set; } = 236;

    public double DetailsWidth { get; set; } = 292;

    public bool IsNavigationCollapsed { get; set; }
}