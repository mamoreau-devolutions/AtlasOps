namespace AtlasOps.Modules.Governance.Core;

using System.Collections.Generic;

public sealed record GovernanceCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class GovernanceModule
{
    public const string Id = "Governance";
    public const string DisplayName = "Governance and policy";
    public static IReadOnlyList<GovernanceCapabilityDescriptor> Capabilities { get; } = new GovernanceCapabilityDescriptor[]
    {
        new("Governance.PolicyAuthoring", "Policy authoring", "Policy", "Authoring", "Coordinates governance and policy for Policy authoring."),
        new("Governance.PolicyEvaluation", "Policy evaluation", "Policy", "Evaluation", "Coordinates governance and policy for Policy evaluation."),
        new("Governance.PolicyApproval", "Policy approval", "Policy", "Approval", "Coordinates governance and policy for Policy approval."),
        new("Governance.PolicyEnforcement", "Policy enforcement", "Policy", "Enforcement", "Coordinates governance and policy for Policy enforcement."),
        new("Governance.PolicyRemediation", "Policy remediation", "Policy", "Remediation", "Coordinates governance and policy for Policy remediation."),
        new("Governance.PolicyAudit", "Policy audit", "Policy", "Audit", "Coordinates governance and policy for Policy audit."),
        new("Governance.ControlAuthoring", "Control authoring", "Control", "Authoring", "Coordinates governance and policy for Control authoring."),
        new("Governance.ControlEvaluation", "Control evaluation", "Control", "Evaluation", "Coordinates governance and policy for Control evaluation."),
        new("Governance.ControlApproval", "Control approval", "Control", "Approval", "Coordinates governance and policy for Control approval."),
        new("Governance.ControlEnforcement", "Control enforcement", "Control", "Enforcement", "Coordinates governance and policy for Control enforcement."),
        new("Governance.ControlRemediation", "Control remediation", "Control", "Remediation", "Coordinates governance and policy for Control remediation."),
        new("Governance.ControlAudit", "Control audit", "Control", "Audit", "Coordinates governance and policy for Control audit."),
        new("Governance.ExceptionAuthoring", "Exception authoring", "Exception", "Authoring", "Coordinates governance and policy for Exception authoring."),
        new("Governance.ExceptionEvaluation", "Exception evaluation", "Exception", "Evaluation", "Coordinates governance and policy for Exception evaluation."),
        new("Governance.ExceptionApproval", "Exception approval", "Exception", "Approval", "Coordinates governance and policy for Exception approval."),
        new("Governance.ExceptionEnforcement", "Exception enforcement", "Exception", "Enforcement", "Coordinates governance and policy for Exception enforcement."),
        new("Governance.ExceptionRemediation", "Exception remediation", "Exception", "Remediation", "Coordinates governance and policy for Exception remediation."),
        new("Governance.ExceptionAudit", "Exception audit", "Exception", "Audit", "Coordinates governance and policy for Exception audit."),
        new("Governance.EvidenceAuthoring", "Evidence authoring", "Evidence", "Authoring", "Coordinates governance and policy for Evidence authoring."),
        new("Governance.EvidenceEvaluation", "Evidence evaluation", "Evidence", "Evaluation", "Coordinates governance and policy for Evidence evaluation."),
        new("Governance.EvidenceApproval", "Evidence approval", "Evidence", "Approval", "Coordinates governance and policy for Evidence approval."),
        new("Governance.EvidenceEnforcement", "Evidence enforcement", "Evidence", "Enforcement", "Coordinates governance and policy for Evidence enforcement."),
        new("Governance.EvidenceRemediation", "Evidence remediation", "Evidence", "Remediation", "Coordinates governance and policy for Evidence remediation."),
        new("Governance.EvidenceAudit", "Evidence audit", "Evidence", "Audit", "Coordinates governance and policy for Evidence audit."),
        new("Governance.AssessmentAuthoring", "Assessment authoring", "Assessment", "Authoring", "Coordinates governance and policy for Assessment authoring."),
        new("Governance.AssessmentEvaluation", "Assessment evaluation", "Assessment", "Evaluation", "Coordinates governance and policy for Assessment evaluation."),
        new("Governance.AssessmentApproval", "Assessment approval", "Assessment", "Approval", "Coordinates governance and policy for Assessment approval."),
        new("Governance.AssessmentEnforcement", "Assessment enforcement", "Assessment", "Enforcement", "Coordinates governance and policy for Assessment enforcement."),
        new("Governance.AssessmentRemediation", "Assessment remediation", "Assessment", "Remediation", "Coordinates governance and policy for Assessment remediation."),
        new("Governance.AssessmentAudit", "Assessment audit", "Assessment", "Audit", "Coordinates governance and policy for Assessment audit."),
    };
}