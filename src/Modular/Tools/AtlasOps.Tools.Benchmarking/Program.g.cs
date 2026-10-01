namespace AtlasOps.Tools.Benchmarking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public sealed record BenchmarkingInput(string Command, IReadOnlyList<string> Arguments, IReadOnlyDictionary<string, string> Options);
public sealed record BenchmarkingFinding(string Code, string Severity, string Subject, string Message, IReadOnlyDictionary<string, string> Evidence);
public sealed record BenchmarkingReport(string Tool, DateTimeOffset StartedAt, DateTimeOffset CompletedAt, IReadOnlyList<BenchmarkingFinding> Findings, IReadOnlyDictionary<string, double> Metrics);
public sealed class BenchmarkingEngine
{
    public BenchmarkingReport Execute(BenchmarkingInput input, DateTimeOffset now)
    {
        List<BenchmarkingFinding> findings = new(); Dictionary<string, double> metrics = new(StringComparer.OrdinalIgnoreCase) { ["argumentCount"] = input.Arguments.Count, ["optionCount"] = input.Options.Count, ["uniqueArgumentCount"] = input.Arguments.Distinct(StringComparer.OrdinalIgnoreCase).Count() };
        if (string.IsNullOrWhiteSpace(input.Command)) { findings.Add(new("missing-command", "error", "input", "A command is required.", new Dictionary<string, string>())); }
        foreach (string duplicate in input.Arguments.GroupBy(static item => item, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1).Select(static group => group.Key)) { findings.Add(new("duplicate-argument", "warning", duplicate, "The argument occurs more than once.", new Dictionary<string, string>())); }
        return new("Benchmarking", now, now.AddMilliseconds(Math.Max(1, input.Arguments.Count)), findings, metrics);
    }
}
public static class Program
{
    public static int Main(string[] args)
    {
        BenchmarkingReport report = new BenchmarkingEngine().Execute(new(args.FirstOrDefault() ?? string.Empty, args.Skip(1).ToArray(), new Dictionary<string, string>()), DateTimeOffset.UtcNow);
        Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        return report.Findings.Any(static finding => finding.Severity == "error") ? 1 : 0;
    }
}