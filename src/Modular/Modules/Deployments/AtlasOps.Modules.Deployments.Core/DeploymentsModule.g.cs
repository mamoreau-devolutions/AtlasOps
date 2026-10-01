namespace AtlasOps.Modules.Deployments.Core;

using System.Collections.Generic;

public sealed record DeploymentsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class DeploymentsModule
{
    public const string Id = "Deployments";
    public const string DisplayName = "Deployments and environments";
    public static IReadOnlyList<DeploymentsCapabilityDescriptor> Capabilities { get; } = new DeploymentsCapabilityDescriptor[]
    {
        new("Deployments.ReleasePlanning", "Release planning", "Release", "Planning", "Coordinates deployments and environments for Release planning."),
        new("Deployments.ReleaseValidation", "Release validation", "Release", "Validation", "Coordinates deployments and environments for Release validation."),
        new("Deployments.ReleasePromotion", "Release promotion", "Release", "Promotion", "Coordinates deployments and environments for Release promotion."),
        new("Deployments.ReleaseMonitoring", "Release monitoring", "Release", "Monitoring", "Coordinates deployments and environments for Release monitoring."),
        new("Deployments.ReleaseRecovery", "Release recovery", "Release", "Recovery", "Coordinates deployments and environments for Release recovery."),
        new("Deployments.ReleaseAudit", "Release audit", "Release", "Audit", "Coordinates deployments and environments for Release audit."),
        new("Deployments.EnvironmentPlanning", "Environment planning", "Environment", "Planning", "Coordinates deployments and environments for Environment planning."),
        new("Deployments.EnvironmentValidation", "Environment validation", "Environment", "Validation", "Coordinates deployments and environments for Environment validation."),
        new("Deployments.EnvironmentPromotion", "Environment promotion", "Environment", "Promotion", "Coordinates deployments and environments for Environment promotion."),
        new("Deployments.EnvironmentMonitoring", "Environment monitoring", "Environment", "Monitoring", "Coordinates deployments and environments for Environment monitoring."),
        new("Deployments.EnvironmentRecovery", "Environment recovery", "Environment", "Recovery", "Coordinates deployments and environments for Environment recovery."),
        new("Deployments.EnvironmentAudit", "Environment audit", "Environment", "Audit", "Coordinates deployments and environments for Environment audit."),
        new("Deployments.ArtifactPlanning", "Artifact planning", "Artifact", "Planning", "Coordinates deployments and environments for Artifact planning."),
        new("Deployments.ArtifactValidation", "Artifact validation", "Artifact", "Validation", "Coordinates deployments and environments for Artifact validation."),
        new("Deployments.ArtifactPromotion", "Artifact promotion", "Artifact", "Promotion", "Coordinates deployments and environments for Artifact promotion."),
        new("Deployments.ArtifactMonitoring", "Artifact monitoring", "Artifact", "Monitoring", "Coordinates deployments and environments for Artifact monitoring."),
        new("Deployments.ArtifactRecovery", "Artifact recovery", "Artifact", "Recovery", "Coordinates deployments and environments for Artifact recovery."),
        new("Deployments.ArtifactAudit", "Artifact audit", "Artifact", "Audit", "Coordinates deployments and environments for Artifact audit."),
        new("Deployments.RolloutPlanning", "Rollout planning", "Rollout", "Planning", "Coordinates deployments and environments for Rollout planning."),
        new("Deployments.RolloutValidation", "Rollout validation", "Rollout", "Validation", "Coordinates deployments and environments for Rollout validation."),
        new("Deployments.RolloutPromotion", "Rollout promotion", "Rollout", "Promotion", "Coordinates deployments and environments for Rollout promotion."),
        new("Deployments.RolloutMonitoring", "Rollout monitoring", "Rollout", "Monitoring", "Coordinates deployments and environments for Rollout monitoring."),
        new("Deployments.RolloutRecovery", "Rollout recovery", "Rollout", "Recovery", "Coordinates deployments and environments for Rollout recovery."),
        new("Deployments.RolloutAudit", "Rollout audit", "Rollout", "Audit", "Coordinates deployments and environments for Rollout audit."),
        new("Deployments.RollbackPlanning", "Rollback planning", "Rollback", "Planning", "Coordinates deployments and environments for Rollback planning."),
        new("Deployments.RollbackValidation", "Rollback validation", "Rollback", "Validation", "Coordinates deployments and environments for Rollback validation."),
        new("Deployments.RollbackPromotion", "Rollback promotion", "Rollback", "Promotion", "Coordinates deployments and environments for Rollback promotion."),
        new("Deployments.RollbackMonitoring", "Rollback monitoring", "Rollback", "Monitoring", "Coordinates deployments and environments for Rollback monitoring."),
        new("Deployments.RollbackRecovery", "Rollback recovery", "Rollback", "Recovery", "Coordinates deployments and environments for Rollback recovery."),
        new("Deployments.RollbackAudit", "Rollback audit", "Rollback", "Audit", "Coordinates deployments and environments for Rollback audit."),
    };
}