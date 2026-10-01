namespace AtlasOps.Modules.Connections.Core;

using System.Collections.Generic;

public sealed record ConnectionsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class ConnectionsModule
{
    public const string Id = "Connections";
    public const string DisplayName = "Connections and sessions";
    public static IReadOnlyList<ConnectionsCapabilityDescriptor> Capabilities { get; } = new ConnectionsCapabilityDescriptor[]
    {
        new("Connections.ProfileProvisioning", "Profile provisioning", "Profile", "Provisioning", "Coordinates connections and sessions for Profile provisioning."),
        new("Connections.ProfileRouting", "Profile routing", "Profile", "Routing", "Coordinates connections and sessions for Profile routing."),
        new("Connections.ProfileHealth", "Profile health", "Profile", "Health", "Coordinates connections and sessions for Profile health."),
        new("Connections.ProfileRecovery", "Profile recovery", "Profile", "Recovery", "Coordinates connections and sessions for Profile recovery."),
        new("Connections.ProfilePolicy", "Profile policy", "Profile", "Policy", "Coordinates connections and sessions for Profile policy."),
        new("Connections.ProfileAudit", "Profile audit", "Profile", "Audit", "Coordinates connections and sessions for Profile audit."),
        new("Connections.SessionProvisioning", "Session provisioning", "Session", "Provisioning", "Coordinates connections and sessions for Session provisioning."),
        new("Connections.SessionRouting", "Session routing", "Session", "Routing", "Coordinates connections and sessions for Session routing."),
        new("Connections.SessionHealth", "Session health", "Session", "Health", "Coordinates connections and sessions for Session health."),
        new("Connections.SessionRecovery", "Session recovery", "Session", "Recovery", "Coordinates connections and sessions for Session recovery."),
        new("Connections.SessionPolicy", "Session policy", "Session", "Policy", "Coordinates connections and sessions for Session policy."),
        new("Connections.SessionAudit", "Session audit", "Session", "Audit", "Coordinates connections and sessions for Session audit."),
        new("Connections.GatewayProvisioning", "Gateway provisioning", "Gateway", "Provisioning", "Coordinates connections and sessions for Gateway provisioning."),
        new("Connections.GatewayRouting", "Gateway routing", "Gateway", "Routing", "Coordinates connections and sessions for Gateway routing."),
        new("Connections.GatewayHealth", "Gateway health", "Gateway", "Health", "Coordinates connections and sessions for Gateway health."),
        new("Connections.GatewayRecovery", "Gateway recovery", "Gateway", "Recovery", "Coordinates connections and sessions for Gateway recovery."),
        new("Connections.GatewayPolicy", "Gateway policy", "Gateway", "Policy", "Coordinates connections and sessions for Gateway policy."),
        new("Connections.GatewayAudit", "Gateway audit", "Gateway", "Audit", "Coordinates connections and sessions for Gateway audit."),
        new("Connections.TunnelProvisioning", "Tunnel provisioning", "Tunnel", "Provisioning", "Coordinates connections and sessions for Tunnel provisioning."),
        new("Connections.TunnelRouting", "Tunnel routing", "Tunnel", "Routing", "Coordinates connections and sessions for Tunnel routing."),
        new("Connections.TunnelHealth", "Tunnel health", "Tunnel", "Health", "Coordinates connections and sessions for Tunnel health."),
        new("Connections.TunnelRecovery", "Tunnel recovery", "Tunnel", "Recovery", "Coordinates connections and sessions for Tunnel recovery."),
        new("Connections.TunnelPolicy", "Tunnel policy", "Tunnel", "Policy", "Coordinates connections and sessions for Tunnel policy."),
        new("Connections.TunnelAudit", "Tunnel audit", "Tunnel", "Audit", "Coordinates connections and sessions for Tunnel audit."),
        new("Connections.EndpointProvisioning", "Endpoint provisioning", "Endpoint", "Provisioning", "Coordinates connections and sessions for Endpoint provisioning."),
        new("Connections.EndpointRouting", "Endpoint routing", "Endpoint", "Routing", "Coordinates connections and sessions for Endpoint routing."),
        new("Connections.EndpointHealth", "Endpoint health", "Endpoint", "Health", "Coordinates connections and sessions for Endpoint health."),
        new("Connections.EndpointRecovery", "Endpoint recovery", "Endpoint", "Recovery", "Coordinates connections and sessions for Endpoint recovery."),
        new("Connections.EndpointPolicy", "Endpoint policy", "Endpoint", "Policy", "Coordinates connections and sessions for Endpoint policy."),
        new("Connections.EndpointAudit", "Endpoint audit", "Endpoint", "Audit", "Coordinates connections and sessions for Endpoint audit."),
    };
}