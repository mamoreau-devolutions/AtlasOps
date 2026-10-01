namespace AtlasOps.Modules.Compliance.Core;

using System.Collections.Generic;

public sealed record ComplianceCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class ComplianceModule
{
    public const string Id = "Compliance";
    public const string DisplayName = "Licensing and compliance";
    public static IReadOnlyList<ComplianceCapabilityDescriptor> Capabilities { get; } = new ComplianceCapabilityDescriptor[]
    {
        new("Compliance.LicenseCatalog", "License catalog", "License", "Catalog", "Coordinates licensing and compliance for License catalog."),
        new("Compliance.LicenseDetection", "License detection", "License", "Detection", "Coordinates licensing and compliance for License detection."),
        new("Compliance.LicenseEvaluation", "License evaluation", "License", "Evaluation", "Coordinates licensing and compliance for License evaluation."),
        new("Compliance.LicenseApproval", "License approval", "License", "Approval", "Coordinates licensing and compliance for License approval."),
        new("Compliance.LicenseRemediation", "License remediation", "License", "Remediation", "Coordinates licensing and compliance for License remediation."),
        new("Compliance.LicenseReporting", "License reporting", "License", "Reporting", "Coordinates licensing and compliance for License reporting."),
        new("Compliance.ObligationCatalog", "Obligation catalog", "Obligation", "Catalog", "Coordinates licensing and compliance for Obligation catalog."),
        new("Compliance.ObligationDetection", "Obligation detection", "Obligation", "Detection", "Coordinates licensing and compliance for Obligation detection."),
        new("Compliance.ObligationEvaluation", "Obligation evaluation", "Obligation", "Evaluation", "Coordinates licensing and compliance for Obligation evaluation."),
        new("Compliance.ObligationApproval", "Obligation approval", "Obligation", "Approval", "Coordinates licensing and compliance for Obligation approval."),
        new("Compliance.ObligationRemediation", "Obligation remediation", "Obligation", "Remediation", "Coordinates licensing and compliance for Obligation remediation."),
        new("Compliance.ObligationReporting", "Obligation reporting", "Obligation", "Reporting", "Coordinates licensing and compliance for Obligation reporting."),
        new("Compliance.ComponentCatalog", "Component catalog", "Component", "Catalog", "Coordinates licensing and compliance for Component catalog."),
        new("Compliance.ComponentDetection", "Component detection", "Component", "Detection", "Coordinates licensing and compliance for Component detection."),
        new("Compliance.ComponentEvaluation", "Component evaluation", "Component", "Evaluation", "Coordinates licensing and compliance for Component evaluation."),
        new("Compliance.ComponentApproval", "Component approval", "Component", "Approval", "Coordinates licensing and compliance for Component approval."),
        new("Compliance.ComponentRemediation", "Component remediation", "Component", "Remediation", "Coordinates licensing and compliance for Component remediation."),
        new("Compliance.ComponentReporting", "Component reporting", "Component", "Reporting", "Coordinates licensing and compliance for Component reporting."),
        new("Compliance.NoticeCatalog", "Notice catalog", "Notice", "Catalog", "Coordinates licensing and compliance for Notice catalog."),
        new("Compliance.NoticeDetection", "Notice detection", "Notice", "Detection", "Coordinates licensing and compliance for Notice detection."),
        new("Compliance.NoticeEvaluation", "Notice evaluation", "Notice", "Evaluation", "Coordinates licensing and compliance for Notice evaluation."),
        new("Compliance.NoticeApproval", "Notice approval", "Notice", "Approval", "Coordinates licensing and compliance for Notice approval."),
        new("Compliance.NoticeRemediation", "Notice remediation", "Notice", "Remediation", "Coordinates licensing and compliance for Notice remediation."),
        new("Compliance.NoticeReporting", "Notice reporting", "Notice", "Reporting", "Coordinates licensing and compliance for Notice reporting."),
        new("Compliance.FindingCatalog", "Finding catalog", "Finding", "Catalog", "Coordinates licensing and compliance for Finding catalog."),
        new("Compliance.FindingDetection", "Finding detection", "Finding", "Detection", "Coordinates licensing and compliance for Finding detection."),
        new("Compliance.FindingEvaluation", "Finding evaluation", "Finding", "Evaluation", "Coordinates licensing and compliance for Finding evaluation."),
        new("Compliance.FindingApproval", "Finding approval", "Finding", "Approval", "Coordinates licensing and compliance for Finding approval."),
        new("Compliance.FindingRemediation", "Finding remediation", "Finding", "Remediation", "Coordinates licensing and compliance for Finding remediation."),
        new("Compliance.FindingReporting", "Finding reporting", "Finding", "Reporting", "Coordinates licensing and compliance for Finding reporting."),
    };
}