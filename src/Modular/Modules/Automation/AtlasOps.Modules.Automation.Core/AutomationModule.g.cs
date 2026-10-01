namespace AtlasOps.Modules.Automation.Core;

using System.Collections.Generic;

public sealed record AutomationCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class AutomationModule
{
    public const string Id = "Automation";
    public const string DisplayName = "Automation and runbooks";
    public static IReadOnlyList<AutomationCapabilityDescriptor> Capabilities { get; } = new AutomationCapabilityDescriptor[]
    {
        new("Automation.RunbookDesign", "Runbook design", "Runbook", "Design", "Coordinates automation and runbooks for Runbook design."),
        new("Automation.RunbookScheduling", "Runbook scheduling", "Runbook", "Scheduling", "Coordinates automation and runbooks for Runbook scheduling."),
        new("Automation.RunbookExecution", "Runbook execution", "Runbook", "Execution", "Coordinates automation and runbooks for Runbook execution."),
        new("Automation.RunbookRetry", "Runbook retry", "Runbook", "Retry", "Coordinates automation and runbooks for Runbook retry."),
        new("Automation.RunbookCancellation", "Runbook cancellation", "Runbook", "Cancellation", "Coordinates automation and runbooks for Runbook cancellation."),
        new("Automation.RunbookAudit", "Runbook audit", "Runbook", "Audit", "Coordinates automation and runbooks for Runbook audit."),
        new("Automation.WorkflowDesign", "Workflow design", "Workflow", "Design", "Coordinates automation and runbooks for Workflow design."),
        new("Automation.WorkflowScheduling", "Workflow scheduling", "Workflow", "Scheduling", "Coordinates automation and runbooks for Workflow scheduling."),
        new("Automation.WorkflowExecution", "Workflow execution", "Workflow", "Execution", "Coordinates automation and runbooks for Workflow execution."),
        new("Automation.WorkflowRetry", "Workflow retry", "Workflow", "Retry", "Coordinates automation and runbooks for Workflow retry."),
        new("Automation.WorkflowCancellation", "Workflow cancellation", "Workflow", "Cancellation", "Coordinates automation and runbooks for Workflow cancellation."),
        new("Automation.WorkflowAudit", "Workflow audit", "Workflow", "Audit", "Coordinates automation and runbooks for Workflow audit."),
        new("Automation.JobDesign", "Job design", "Job", "Design", "Coordinates automation and runbooks for Job design."),
        new("Automation.JobScheduling", "Job scheduling", "Job", "Scheduling", "Coordinates automation and runbooks for Job scheduling."),
        new("Automation.JobExecution", "Job execution", "Job", "Execution", "Coordinates automation and runbooks for Job execution."),
        new("Automation.JobRetry", "Job retry", "Job", "Retry", "Coordinates automation and runbooks for Job retry."),
        new("Automation.JobCancellation", "Job cancellation", "Job", "Cancellation", "Coordinates automation and runbooks for Job cancellation."),
        new("Automation.JobAudit", "Job audit", "Job", "Audit", "Coordinates automation and runbooks for Job audit."),
        new("Automation.TriggerDesign", "Trigger design", "Trigger", "Design", "Coordinates automation and runbooks for Trigger design."),
        new("Automation.TriggerScheduling", "Trigger scheduling", "Trigger", "Scheduling", "Coordinates automation and runbooks for Trigger scheduling."),
        new("Automation.TriggerExecution", "Trigger execution", "Trigger", "Execution", "Coordinates automation and runbooks for Trigger execution."),
        new("Automation.TriggerRetry", "Trigger retry", "Trigger", "Retry", "Coordinates automation and runbooks for Trigger retry."),
        new("Automation.TriggerCancellation", "Trigger cancellation", "Trigger", "Cancellation", "Coordinates automation and runbooks for Trigger cancellation."),
        new("Automation.TriggerAudit", "Trigger audit", "Trigger", "Audit", "Coordinates automation and runbooks for Trigger audit."),
        new("Automation.ApprovalDesign", "Approval design", "Approval", "Design", "Coordinates automation and runbooks for Approval design."),
        new("Automation.ApprovalScheduling", "Approval scheduling", "Approval", "Scheduling", "Coordinates automation and runbooks for Approval scheduling."),
        new("Automation.ApprovalExecution", "Approval execution", "Approval", "Execution", "Coordinates automation and runbooks for Approval execution."),
        new("Automation.ApprovalRetry", "Approval retry", "Approval", "Retry", "Coordinates automation and runbooks for Approval retry."),
        new("Automation.ApprovalCancellation", "Approval cancellation", "Approval", "Cancellation", "Coordinates automation and runbooks for Approval cancellation."),
        new("Automation.ApprovalAudit", "Approval audit", "Approval", "Audit", "Coordinates automation and runbooks for Approval audit."),
    };
}