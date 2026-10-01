namespace AtlasOps.Composition;

using System;
using System.Collections.Generic;

using global::Avalonia.Controls;

using AtlasOps.Enterprise.Avalonia.Incidents;
using AtlasOps.Enterprise.Avalonia.Inventory;
using AtlasOps.Enterprise.Avalonia.Policy;
using AtlasOps.Enterprise.Avalonia.Reporting;
using AtlasOps.Enterprise.Avalonia.Workflow;
using AtlasOps.Modules.Workspaces.Avalonia;
using AtlasOps.Modules.Connections.Avalonia;
using AtlasOps.Modules.Credentials.Avalonia;
using AtlasOps.Modules.Inventory.Avalonia;
using AtlasOps.Modules.Automation.Avalonia;
using AtlasOps.Modules.Deployments.Avalonia;
using AtlasOps.Modules.Incidents.Avalonia;
using AtlasOps.Modules.Observability.Avalonia;
using AtlasOps.Modules.Governance.Avalonia;
using AtlasOps.Modules.Identity.Avalonia;
using AtlasOps.Modules.Documents.Avalonia;
using AtlasOps.Modules.Geography.Avalonia;
using AtlasOps.Modules.NetworkIntelligence.Avalonia;
using AtlasOps.Modules.Lifecycle.Avalonia;
using AtlasOps.Modules.Localization.Avalonia;
using AtlasOps.Modules.CloudEconomics.Avalonia;
using AtlasOps.Modules.Geospatial.Avalonia;
using AtlasOps.Modules.Compliance.Avalonia;
using AtlasOps.Modules.Workspaces.Core;
using AtlasOps.Modules.Connections.Core;
using AtlasOps.Modules.Credentials.Core;
using AtlasOps.Modules.Inventory.Core;
using AtlasOps.Modules.Automation.Core;
using AtlasOps.Modules.Deployments.Core;
using AtlasOps.Modules.Incidents.Core;
using AtlasOps.Modules.Observability.Core;
using AtlasOps.Modules.Governance.Core;
using AtlasOps.Modules.Identity.Core;
using AtlasOps.Modules.Documents.Core;
using AtlasOps.Modules.Geography.Core;
using AtlasOps.Modules.NetworkIntelligence.Core;
using AtlasOps.Modules.Lifecycle.Core;
using AtlasOps.Modules.Localization.Core;
using AtlasOps.Modules.CloudEconomics.Core;
using AtlasOps.Modules.Geospatial.Core;
using AtlasOps.Modules.Compliance.Core;

public sealed record AtlasOpsModuleDescriptor(string Id, string DisplayName, int CapabilityCount, Func<UserControl> CreateView);
public static class AtlasOpsModuleCatalog
{
    public static IReadOnlyList<AtlasOpsModuleDescriptor> Create()
    {
        return new AtlasOpsModuleDescriptor[]
        {
        new("Workspaces", "Workspaces and projects", WorkspacesModule.Capabilities.Count, static () => new WorkspacesWorkbenchView()),
        new("Connections", "Connections and sessions", ConnectionsModule.Capabilities.Count, static () => new ConnectionsWorkbenchView()),
        new("Credentials", "Credentials and secrets", CredentialsModule.Capabilities.Count, static () => new CredentialsWorkbenchView()),
        new("Inventory", "Assets and inventory", InventoryModule.Capabilities.Count, static () => new InventoryStudioView()),
        new("Automation", "Automation and runbooks", AutomationModule.Capabilities.Count, static () => new WorkflowStudioView()),
        new("Deployments", "Deployments and environments", DeploymentsModule.Capabilities.Count, static () => new DeploymentsWorkbenchView()),
        new("Incidents", "Incidents and alerting", IncidentsModule.Capabilities.Count, static () => new IncidentStudioView()),
        new("Observability", "Observability and metrics", ObservabilityModule.Capabilities.Count, static () => new ReportingStudioView()),
        new("Governance", "Governance and policy", GovernanceModule.Capabilities.Count, static () => new PolicyStudioView()),
        new("Identity", "Identity and organization", IdentityModule.Capabilities.Count, static () => new IdentityWorkbenchView()),
        new("Documents", "Documents and knowledge", DocumentsModule.Capabilities.Count, static () => new DocumentsWorkbenchView()),
        new("Geography", "Geography intelligence", GeographyModule.Capabilities.Count, static () => new GeographyWorkbenchView()),
        new("NetworkIntelligence", "IANA network intelligence", NetworkIntelligenceModule.Capabilities.Count, static () => new NetworkIntelligenceWorkbenchView()),
        new("Lifecycle", "Software lifecycle", LifecycleModule.Capabilities.Count, static () => new LifecycleWorkbenchView()),
        new("Localization", "Localization and time zones", LocalizationModule.Capabilities.Count, static () => new LocalizationWorkbenchView()),
        new("CloudEconomics", "Cloud economics", CloudEconomicsModule.Capabilities.Count, static () => new CloudEconomicsWorkbenchView()),
        new("Geospatial", "Geospatial operations", GeospatialModule.Capabilities.Count, static () => new GeospatialWorkbenchView()),
        new("Compliance", "Licensing and compliance", ComplianceModule.Capabilities.Count, static () => new ComplianceWorkbenchView()),
        };
    }
}