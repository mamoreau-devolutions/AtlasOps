namespace AtlasOps.Tools.ProjectGraph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record ProjectGraphInput(string Command, IReadOnlyList<string> Arguments, IReadOnlyDictionary<string, string> Options);
public sealed record ProjectGraphFinding(string Code, string Severity, string Subject, string Message, IReadOnlyDictionary<string, string> Evidence);
public sealed record ProjectGraphReport(string Tool, DateTimeOffset StartedAt, DateTimeOffset CompletedAt, IReadOnlyList<ProjectGraphFinding> Findings, IReadOnlyDictionary<string, double> Metrics);
public sealed class ProjectGraphEngine
{
    public ProjectGraphReport Execute(ProjectGraphInput input, DateTimeOffset now)
    {
        List<ProjectGraphFinding> findings = new(); Dictionary<string, double> metrics = new(StringComparer.OrdinalIgnoreCase) { ["argumentCount"] = input.Arguments.Count, ["optionCount"] = input.Options.Count, ["uniqueArgumentCount"] = input.Arguments.Distinct(StringComparer.OrdinalIgnoreCase).Count() };
        if (string.IsNullOrWhiteSpace(input.Command)) { findings.Add(new("missing-command", "error", "input", "A command is required.", new Dictionary<string, string>())); }
        foreach (string duplicate in input.Arguments.GroupBy(static item => item, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1).Select(static group => group.Key)) { findings.Add(new("duplicate-argument", "warning", duplicate, "The argument occurs more than once.", new Dictionary<string, string>())); }
        return new("ProjectGraph", now, now.AddMilliseconds(Math.Max(1, input.Arguments.Count)), findings, metrics);
    }
}
public static class Program
{
    public static int Main(string[] args)
    {
        ProjectGraphReport report = new ProjectGraphEngine().Execute(new(args.FirstOrDefault() ?? string.Empty, args.Skip(1).ToArray(), new Dictionary<string, string>()), DateTimeOffset.UtcNow);
        Console.WriteLine(JsonSerializer.Serialize(report, ProjectGraphJsonContext.Default.ProjectGraphReport));
        return report.Findings.Any(static finding => finding.Severity == "error") ? 1 : 0;
    }
}
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ProjectGraphReport))]
internal sealed partial class ProjectGraphJsonContext : JsonSerializerContext
{
}