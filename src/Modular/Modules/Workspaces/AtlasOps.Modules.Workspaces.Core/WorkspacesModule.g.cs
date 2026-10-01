namespace AtlasOps.Modules.Workspaces.Core;

using System.Collections.Generic;

public sealed record WorkspacesCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class WorkspacesModule
{
    public const string Id = "Workspaces";
    public const string DisplayName = "Workspaces and projects";
    public static IReadOnlyList<WorkspacesCapabilityDescriptor> Capabilities { get; } = new WorkspacesCapabilityDescriptor[]
    {
        new("Workspaces.PortfolioLifecycle", "Portfolio lifecycle", "Portfolio", "Lifecycle", "Coordinates workspaces and projects for Portfolio lifecycle."),
        new("Workspaces.PortfolioGovernance", "Portfolio governance", "Portfolio", "Governance", "Coordinates workspaces and projects for Portfolio governance."),
        new("Workspaces.PortfolioCollaboration", "Portfolio collaboration", "Portfolio", "Collaboration", "Coordinates workspaces and projects for Portfolio collaboration."),
        new("Workspaces.PortfolioForecasting", "Portfolio forecasting", "Portfolio", "Forecasting", "Coordinates workspaces and projects for Portfolio forecasting."),
        new("Workspaces.PortfolioReporting", "Portfolio reporting", "Portfolio", "Reporting", "Coordinates workspaces and projects for Portfolio reporting."),
        new("Workspaces.PortfolioArchival", "Portfolio archival", "Portfolio", "Archival", "Coordinates workspaces and projects for Portfolio archival."),
        new("Workspaces.WorkspaceLifecycle", "Workspace lifecycle", "Workspace", "Lifecycle", "Coordinates workspaces and projects for Workspace lifecycle."),
        new("Workspaces.WorkspaceGovernance", "Workspace governance", "Workspace", "Governance", "Coordinates workspaces and projects for Workspace governance."),
        new("Workspaces.WorkspaceCollaboration", "Workspace collaboration", "Workspace", "Collaboration", "Coordinates workspaces and projects for Workspace collaboration."),
        new("Workspaces.WorkspaceForecasting", "Workspace forecasting", "Workspace", "Forecasting", "Coordinates workspaces and projects for Workspace forecasting."),
        new("Workspaces.WorkspaceReporting", "Workspace reporting", "Workspace", "Reporting", "Coordinates workspaces and projects for Workspace reporting."),
        new("Workspaces.WorkspaceArchival", "Workspace archival", "Workspace", "Archival", "Coordinates workspaces and projects for Workspace archival."),
        new("Workspaces.ProjectLifecycle", "Project lifecycle", "Project", "Lifecycle", "Coordinates workspaces and projects for Project lifecycle."),
        new("Workspaces.ProjectGovernance", "Project governance", "Project", "Governance", "Coordinates workspaces and projects for Project governance."),
        new("Workspaces.ProjectCollaboration", "Project collaboration", "Project", "Collaboration", "Coordinates workspaces and projects for Project collaboration."),
        new("Workspaces.ProjectForecasting", "Project forecasting", "Project", "Forecasting", "Coordinates workspaces and projects for Project forecasting."),
        new("Workspaces.ProjectReporting", "Project reporting", "Project", "Reporting", "Coordinates workspaces and projects for Project reporting."),
        new("Workspaces.ProjectArchival", "Project archival", "Project", "Archival", "Coordinates workspaces and projects for Project archival."),
        new("Workspaces.MilestoneLifecycle", "Milestone lifecycle", "Milestone", "Lifecycle", "Coordinates workspaces and projects for Milestone lifecycle."),
        new("Workspaces.MilestoneGovernance", "Milestone governance", "Milestone", "Governance", "Coordinates workspaces and projects for Milestone governance."),
        new("Workspaces.MilestoneCollaboration", "Milestone collaboration", "Milestone", "Collaboration", "Coordinates workspaces and projects for Milestone collaboration."),
        new("Workspaces.MilestoneForecasting", "Milestone forecasting", "Milestone", "Forecasting", "Coordinates workspaces and projects for Milestone forecasting."),
        new("Workspaces.MilestoneReporting", "Milestone reporting", "Milestone", "Reporting", "Coordinates workspaces and projects for Milestone reporting."),
        new("Workspaces.MilestoneArchival", "Milestone archival", "Milestone", "Archival", "Coordinates workspaces and projects for Milestone archival."),
        new("Workspaces.StakeholderLifecycle", "Stakeholder lifecycle", "Stakeholder", "Lifecycle", "Coordinates workspaces and projects for Stakeholder lifecycle."),
        new("Workspaces.StakeholderGovernance", "Stakeholder governance", "Stakeholder", "Governance", "Coordinates workspaces and projects for Stakeholder governance."),
        new("Workspaces.StakeholderCollaboration", "Stakeholder collaboration", "Stakeholder", "Collaboration", "Coordinates workspaces and projects for Stakeholder collaboration."),
        new("Workspaces.StakeholderForecasting", "Stakeholder forecasting", "Stakeholder", "Forecasting", "Coordinates workspaces and projects for Stakeholder forecasting."),
        new("Workspaces.StakeholderReporting", "Stakeholder reporting", "Stakeholder", "Reporting", "Coordinates workspaces and projects for Stakeholder reporting."),
        new("Workspaces.StakeholderArchival", "Stakeholder archival", "Stakeholder", "Archival", "Coordinates workspaces and projects for Stakeholder archival."),
    };
}