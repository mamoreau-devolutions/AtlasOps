namespace AtlasOps.Enterprise.Contracts.Workflow;

public enum WorkflowStepKind
{
    Action,
    Approval,
    Condition,
    Delay,
    Notification,
    SubWorkflow,
}

public enum WorkflowDiagnosticSeverity
{
    Information,
    Warning,
    Error,
}

public enum WorkflowFailureAction
{
    Stop,
    Retry,
    Compensate,
    Continue,
}

public sealed record WorkflowRetryPolicy(
    int MaximumAttempts,
    TimeSpan InitialDelay,
    double BackoffMultiplier,
    TimeSpan MaximumDelay);

public sealed record WorkflowStep(
    string Id,
    string DisplayName,
    WorkflowStepKind Kind,
    string Handler,
    IReadOnlyDictionary<string, string> Inputs,
    WorkflowRetryPolicy RetryPolicy,
    string? CompensationStepId = null,
    TimeSpan? Timeout = null);

public sealed record WorkflowEdge(
    string SourceStepId,
    string TargetStepId,
    string? Condition = null,
    int Priority = 0);

public sealed record WorkflowDefinition(
    string Id,
    string DisplayName,
    int Version,
    IReadOnlyList<WorkflowStep> Steps,
    IReadOnlyList<WorkflowEdge> Edges,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record WorkflowDiagnostic(
    string Code,
    WorkflowDiagnosticSeverity Severity,
    string Message,
    string? StepId = null);

public sealed record WorkflowValidationResult(
    bool IsValid,
    IReadOnlyList<WorkflowDiagnostic> Diagnostics,
    IReadOnlyList<string> EntryStepIds,
    IReadOnlyList<string> TerminalStepIds);

public sealed record WorkflowExecutionLayer(
    int Index,
    IReadOnlyList<WorkflowStep> Steps);

public sealed record WorkflowExecutionPlan(
    WorkflowDefinition Definition,
    IReadOnlyList<WorkflowExecutionLayer> Layers,
    IReadOnlyDictionary<string, IReadOnlyList<string>> Dependencies,
    IReadOnlyList<WorkflowDiagnostic> Diagnostics);

public sealed record WorkflowConditionContext(
    IReadOnlyDictionary<string, object?> Variables);

public sealed record WorkflowConditionResult(
    bool IsValid,
    bool Value,
    string Explanation);

public sealed record WorkflowRetryAttempt(
    int Attempt,
    TimeSpan Delay,
    DateTimeOffset ScheduledAt);

public sealed record WorkflowFailurePlan(
    string FailedStepId,
    WorkflowFailureAction Action,
    IReadOnlyList<WorkflowRetryAttempt> Retries,
    IReadOnlyList<string> CompensationStepIds,
    string Explanation);
