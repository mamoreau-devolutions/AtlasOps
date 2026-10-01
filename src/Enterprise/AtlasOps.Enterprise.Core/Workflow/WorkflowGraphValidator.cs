namespace AtlasOps.Enterprise.Core.Workflow;

using AtlasOps.Enterprise.Contracts.Workflow;

public sealed class WorkflowGraphValidator
{
    public WorkflowValidationResult Validate(WorkflowDefinition definition)
    {
        List<WorkflowDiagnostic> diagnostics = [];
        Dictionary<string, WorkflowStep> steps = new(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(definition.Id))
        {
            diagnostics.Add(new("workflow.id.required", WorkflowDiagnosticSeverity.Error, "The workflow identifier is required."));
        }

        if (definition.Version < 1)
        {
            diagnostics.Add(new("workflow.version.invalid", WorkflowDiagnosticSeverity.Error, "The workflow version must be greater than zero."));
        }

        foreach (WorkflowStep step in definition.Steps)
        {
            if (string.IsNullOrWhiteSpace(step.Id))
            {
                diagnostics.Add(new("step.id.required", WorkflowDiagnosticSeverity.Error, "Every step requires an identifier."));
                continue;
            }

            if (!steps.TryAdd(step.Id, step))
            {
                diagnostics.Add(new("step.id.duplicate", WorkflowDiagnosticSeverity.Error, $"Step identifier '{step.Id}' is duplicated.", step.Id));
            }

            this.ValidateStep(step, diagnostics);
        }

        Dictionary<string, int> incomingCounts = steps.Keys.ToDictionary(static id => id, static _ => 0, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<string>> outgoing = steps.Keys.ToDictionary(static id => id, static _ => new List<string>(), StringComparer.OrdinalIgnoreCase);

        foreach (WorkflowEdge edge in definition.Edges)
        {
            bool hasSource = steps.ContainsKey(edge.SourceStepId);
            bool hasTarget = steps.ContainsKey(edge.TargetStepId);

            if (!hasSource)
            {
                diagnostics.Add(new("edge.source.missing", WorkflowDiagnosticSeverity.Error, $"Edge source '{edge.SourceStepId}' does not exist.", edge.SourceStepId));
            }

            if (!hasTarget)
            {
                diagnostics.Add(new("edge.target.missing", WorkflowDiagnosticSeverity.Error, $"Edge target '{edge.TargetStepId}' does not exist.", edge.TargetStepId));
            }

            if (hasSource && hasTarget)
            {
                outgoing[edge.SourceStepId].Add(edge.TargetStepId);
                incomingCounts[edge.TargetStepId]++;

                if (string.Equals(edge.SourceStepId, edge.TargetStepId, StringComparison.OrdinalIgnoreCase))
                {
                    diagnostics.Add(new("edge.self.reference", WorkflowDiagnosticSeverity.Error, $"Step '{edge.SourceStepId}' cannot target itself.", edge.SourceStepId));
                }
            }
        }

        HashSet<string> compensationStepIds = definition.Steps
            .Where(static step => !string.IsNullOrWhiteSpace(step.CompensationStepId))
            .Select(static step => step.CompensationStepId!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        string[] entrySteps = incomingCounts
            .Where(pair => pair.Value == 0 && !compensationStepIds.Contains(pair.Key))
            .Select(static pair => pair.Key)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        string[] terminalSteps = outgoing
            .Where(static pair => pair.Value.Count == 0)
            .Select(static pair => pair.Key)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (steps.Count > 0 && entrySteps.Length == 0)
        {
            diagnostics.Add(new("workflow.entry.missing", WorkflowDiagnosticSeverity.Error, "The workflow has no entry step."));
        }

        if (steps.Count > 0 && terminalSteps.Length == 0)
        {
            diagnostics.Add(new("workflow.terminal.missing", WorkflowDiagnosticSeverity.Error, "The workflow has no terminal step."));
        }

        this.ValidateReachability(entrySteps, outgoing, diagnostics);
        this.ValidateCycles(incomingCounts, outgoing, diagnostics);

        bool isValid = diagnostics.All(static diagnostic => diagnostic.Severity != WorkflowDiagnosticSeverity.Error);
        return new(isValid, diagnostics, entrySteps, terminalSteps);
    }

    private void ValidateStep(WorkflowStep step, ICollection<WorkflowDiagnostic> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(step.DisplayName))
        {
            diagnostics.Add(new("step.name.required", WorkflowDiagnosticSeverity.Error, $"Step '{step.Id}' requires a display name.", step.Id));
        }

        if (step.Kind == WorkflowStepKind.Action && string.IsNullOrWhiteSpace(step.Handler))
        {
            diagnostics.Add(new("step.handler.required", WorkflowDiagnosticSeverity.Error, $"Action step '{step.Id}' requires a handler.", step.Id));
        }

        WorkflowRetryPolicy retry = step.RetryPolicy;
        if (retry.MaximumAttempts < 1)
        {
            diagnostics.Add(new("step.retry.attempts", WorkflowDiagnosticSeverity.Error, $"Step '{step.Id}' must allow at least one attempt.", step.Id));
        }

        if (retry.BackoffMultiplier < 1d)
        {
            diagnostics.Add(new("step.retry.backoff", WorkflowDiagnosticSeverity.Error, $"Step '{step.Id}' retry backoff must be at least 1.", step.Id));
        }

        if (retry.InitialDelay < TimeSpan.Zero || retry.MaximumDelay < retry.InitialDelay)
        {
            diagnostics.Add(new("step.retry.delay", WorkflowDiagnosticSeverity.Error, $"Step '{step.Id}' has an invalid retry delay range.", step.Id));
        }
    }

    private void ValidateReachability(
        IReadOnlyList<string> entries,
        IReadOnlyDictionary<string, List<string>> outgoing,
        ICollection<WorkflowDiagnostic> diagnostics)
    {
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        Queue<string> pending = new(entries);

        while (pending.TryDequeue(out string? current))
        {
            if (!visited.Add(current))
            {
                continue;
            }

            foreach (string target in outgoing[current])
            {
                pending.Enqueue(target);
            }
        }

        foreach (string unreachable in outgoing.Keys.Where(id => !visited.Contains(id)).Order(StringComparer.OrdinalIgnoreCase))
        {
            diagnostics.Add(new("step.unreachable", WorkflowDiagnosticSeverity.Warning, $"Step '{unreachable}' cannot be reached from an entry step.", unreachable));
        }
    }

    private void ValidateCycles(
        IReadOnlyDictionary<string, int> incomingCounts,
        IReadOnlyDictionary<string, List<string>> outgoing,
        ICollection<WorkflowDiagnostic> diagnostics)
    {
        Dictionary<string, int> remaining = new(incomingCounts, StringComparer.OrdinalIgnoreCase);
        PriorityQueue<string, string> ready = new(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, int> pair in remaining)
        {
            if (pair.Value == 0)
            {
                ready.Enqueue(pair.Key, pair.Key);
            }
        }

        int visited = 0;
        while (ready.TryDequeue(out string? current, out _))
        {
            visited++;
            foreach (string target in outgoing[current])
            {
                remaining[target]--;
                if (remaining[target] == 0)
                {
                    ready.Enqueue(target, target);
                }
            }
        }

        if (visited != remaining.Count)
        {
            string[] cyclic = remaining
                .Where(static pair => pair.Value > 0)
                .Select(static pair => pair.Key)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            diagnostics.Add(new("workflow.cycle", WorkflowDiagnosticSeverity.Error, $"The workflow contains a cycle involving: {string.Join(", ", cyclic)}."));
        }
    }
}
