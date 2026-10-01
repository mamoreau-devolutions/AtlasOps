namespace AtlasOps.Enterprise.Core.Workflow;

using AtlasOps.Enterprise.Contracts.Workflow;

public sealed class WorkflowPlanner
{
    private readonly WorkflowGraphValidator validator = new();

    public WorkflowExecutionPlan CreatePlan(WorkflowDefinition definition)
    {
        WorkflowValidationResult validation = this.validator.Validate(definition);
        if (!validation.IsValid)
        {
            return new(definition, [], new Dictionary<string, IReadOnlyList<string>>(), validation.Diagnostics);
        }

        HashSet<string> compensationStepIds = definition.Steps
            .Where(static step => !string.IsNullOrWhiteSpace(step.CompensationStepId))
            .Select(static step => step.CompensationStepId!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        HashSet<string> connectedStepIds = definition.Edges
            .SelectMany(static edge => new[] { edge.SourceStepId, edge.TargetStepId })
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, WorkflowStep> steps = definition.Steps
            .Where(step => !compensationStepIds.Contains(step.Id) || connectedStepIds.Contains(step.Id))
            .ToDictionary(static step => step.Id, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<string>> dependencies = steps.Keys.ToDictionary(static id => id, static _ => new List<string>(), StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<string>> outgoing = steps.Keys.ToDictionary(static id => id, static _ => new List<string>(), StringComparer.OrdinalIgnoreCase);

        foreach (WorkflowEdge edge in definition.Edges)
        {
            if (!steps.ContainsKey(edge.SourceStepId) || !steps.ContainsKey(edge.TargetStepId))
            {
                continue;
            }

            dependencies[edge.TargetStepId].Add(edge.SourceStepId);
            outgoing[edge.SourceStepId].Add(edge.TargetStepId);
        }

        Dictionary<string, int> remainingDependencies = dependencies.ToDictionary(
            static pair => pair.Key,
            static pair => pair.Value.Count,
            StringComparer.OrdinalIgnoreCase);
        List<WorkflowExecutionLayer> layers = [];
        HashSet<string> scheduled = new(StringComparer.OrdinalIgnoreCase);
        int layerIndex = 0;

        while (scheduled.Count < steps.Count)
        {
            string[] ready = remainingDependencies
                .Where(pair => pair.Value == 0 && !scheduled.Contains(pair.Key))
                .Select(static pair => pair.Key)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (ready.Length == 0)
            {
                break;
            }

            WorkflowStep[] layerSteps = ready.Select(id => steps[id]).ToArray();
            layers.Add(new(layerIndex++, layerSteps));

            foreach (string stepId in ready)
            {
                scheduled.Add(stepId);
                foreach (string target in outgoing[stepId])
                {
                    remainingDependencies[target]--;
                }
            }
        }

        Dictionary<string, IReadOnlyList<string>> readOnlyDependencies = dependencies.ToDictionary(
            static pair => pair.Key,
            static pair => (IReadOnlyList<string>)pair.Value.Order(StringComparer.OrdinalIgnoreCase).ToArray(),
            StringComparer.OrdinalIgnoreCase);
        return new(definition, layers, readOnlyDependencies, validation.Diagnostics);
    }
}
