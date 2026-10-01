namespace AtlasOps.Modules.Identity.Core;

using System.Collections.Generic;

public sealed record IdentityCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class IdentityModule
{
    public const string Id = "Identity";
    public const string DisplayName = "Identity and organization";
    public static IReadOnlyList<IdentityCapabilityDescriptor> Capabilities { get; } = new IdentityCapabilityDescriptor[]
    {
        new("Identity.IdentityProvisioning", "Identity provisioning", "Identity", "Provisioning", "Coordinates identity and organization for Identity provisioning."),
        new("Identity.IdentityAssignment", "Identity assignment", "Identity", "Assignment", "Coordinates identity and organization for Identity assignment."),
        new("Identity.IdentityDelegation", "Identity delegation", "Identity", "Delegation", "Coordinates identity and organization for Identity delegation."),
        new("Identity.IdentityReview", "Identity review", "Identity", "Review", "Coordinates identity and organization for Identity review."),
        new("Identity.IdentityRevocation", "Identity revocation", "Identity", "Revocation", "Coordinates identity and organization for Identity revocation."),
        new("Identity.IdentityAudit", "Identity audit", "Identity", "Audit", "Coordinates identity and organization for Identity audit."),
        new("Identity.TeamProvisioning", "Team provisioning", "Team", "Provisioning", "Coordinates identity and organization for Team provisioning."),
        new("Identity.TeamAssignment", "Team assignment", "Team", "Assignment", "Coordinates identity and organization for Team assignment."),
        new("Identity.TeamDelegation", "Team delegation", "Team", "Delegation", "Coordinates identity and organization for Team delegation."),
        new("Identity.TeamReview", "Team review", "Team", "Review", "Coordinates identity and organization for Team review."),
        new("Identity.TeamRevocation", "Team revocation", "Team", "Revocation", "Coordinates identity and organization for Team revocation."),
        new("Identity.TeamAudit", "Team audit", "Team", "Audit", "Coordinates identity and organization for Team audit."),
        new("Identity.RoleProvisioning", "Role provisioning", "Role", "Provisioning", "Coordinates identity and organization for Role provisioning."),
        new("Identity.RoleAssignment", "Role assignment", "Role", "Assignment", "Coordinates identity and organization for Role assignment."),
        new("Identity.RoleDelegation", "Role delegation", "Role", "Delegation", "Coordinates identity and organization for Role delegation."),
        new("Identity.RoleReview", "Role review", "Role", "Review", "Coordinates identity and organization for Role review."),
        new("Identity.RoleRevocation", "Role revocation", "Role", "Revocation", "Coordinates identity and organization for Role revocation."),
        new("Identity.RoleAudit", "Role audit", "Role", "Audit", "Coordinates identity and organization for Role audit."),
        new("Identity.MembershipProvisioning", "Membership provisioning", "Membership", "Provisioning", "Coordinates identity and organization for Membership provisioning."),
        new("Identity.MembershipAssignment", "Membership assignment", "Membership", "Assignment", "Coordinates identity and organization for Membership assignment."),
        new("Identity.MembershipDelegation", "Membership delegation", "Membership", "Delegation", "Coordinates identity and organization for Membership delegation."),
        new("Identity.MembershipReview", "Membership review", "Membership", "Review", "Coordinates identity and organization for Membership review."),
        new("Identity.MembershipRevocation", "Membership revocation", "Membership", "Revocation", "Coordinates identity and organization for Membership revocation."),
        new("Identity.MembershipAudit", "Membership audit", "Membership", "Audit", "Coordinates identity and organization for Membership audit."),
        new("Identity.EntitlementProvisioning", "Entitlement provisioning", "Entitlement", "Provisioning", "Coordinates identity and organization for Entitlement provisioning."),
        new("Identity.EntitlementAssignment", "Entitlement assignment", "Entitlement", "Assignment", "Coordinates identity and organization for Entitlement assignment."),
        new("Identity.EntitlementDelegation", "Entitlement delegation", "Entitlement", "Delegation", "Coordinates identity and organization for Entitlement delegation."),
        new("Identity.EntitlementReview", "Entitlement review", "Entitlement", "Review", "Coordinates identity and organization for Entitlement review."),
        new("Identity.EntitlementRevocation", "Entitlement revocation", "Entitlement", "Revocation", "Coordinates identity and organization for Entitlement revocation."),
        new("Identity.EntitlementAudit", "Entitlement audit", "Entitlement", "Audit", "Coordinates identity and organization for Entitlement audit."),
    };
}