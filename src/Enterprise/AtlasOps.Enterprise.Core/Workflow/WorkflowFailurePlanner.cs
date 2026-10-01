namespace AtlasOps.Enterprise.Core.Workflow;

using AtlasOps.Enterprise.Contracts.Workflow;

public sealed class WorkflowFailurePlanner
{
    public WorkflowFailurePlan Plan(WorkflowDefinition definition, string failedStepId, int completedAttempts, DateTimeOffset now)
    {
        WorkflowStep? failedStep = definition.Steps.FirstOrDefault(
            step => string.Equals(step.Id, failedStepId, StringComparison.OrdinalIgnoreCase));
        if (failedStep is null)
        {
            return new(failedStepId, WorkflowFailureAction.Stop, [], [], $"Step '{failedStepId}' does not exist.");
        }

        WorkflowRetryPolicy retry = failedStep.RetryPolicy;
        if (completedAttempts < retry.MaximumAttempts)
        {
            List<WorkflowRetryAttempt> attempts = [];
            TimeSpan cumulativeDelay = TimeSpan.Zero;

            for (int attempt = Math.Max(1, completedAttempts + 1); attempt <= retry.MaximumAttempts; attempt++)
            {
                double exponent = Math.Pow(retry.BackoffMultiplier, attempt - 2);
                double proposedMilliseconds = retry.InitialDelay.TotalMilliseconds * Math.Max(1d, exponent);
                TimeSpan delay = TimeSpan.FromMilliseconds(Math.Min(proposedMilliseconds, retry.MaximumDelay.TotalMilliseconds));
                cumulativeDelay += delay;
                attempts.Add(new(attempt, delay, now + cumulativeDelay));
            }

            return new(failedStepId, WorkflowFailureAction.Retry, attempts, [], $"{attempts.Count} retry attempt(s) remain.");
        }

        if (!string.IsNullOrWhiteSpace(failedStep.CompensationStepId))
        {
            string[] compensation = this.BuildCompensationChain(definition, failedStep.CompensationStepId);
            return new(failedStepId, WorkflowFailureAction.Compensate, [], compensation, "Retry attempts are exhausted; compensation is required.");
        }

        return new(failedStepId, WorkflowFailureAction.Stop, [], [], "Retry attempts are exhausted and no compensation step is configured.");
    }

    private string[] BuildCompensationChain(WorkflowDefinition definition, string firstStepId)
    {
        Dictionary<string, WorkflowStep> steps = definition.Steps.ToDictionary(static step => step.Id, StringComparer.OrdinalIgnoreCase);
        List<string> chain = [];
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        string? current = firstStepId;

        while (!string.IsNullOrWhiteSpace(current) && visited.Add(current) && steps.TryGetValue(current, out WorkflowStep? step))
        {
            chain.Add(step.Id);
            current = step.CompensationStepId;
        }

        return chain.ToArray();
    }
}
