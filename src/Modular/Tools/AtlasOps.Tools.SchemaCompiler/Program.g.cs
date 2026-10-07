namespace AtlasOps.Tools.SchemaCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record SchemaCompilerInput(string Command, IReadOnlyList<string> Arguments, IReadOnlyDictionary<string, string> Options);
public sealed record SchemaCompilerFinding(string Code, string Severity, string Subject, string Message, IReadOnlyDictionary<string, string> Evidence);
public sealed record SchemaCompilerReport(string Tool, DateTimeOffset StartedAt, DateTimeOffset CompletedAt, IReadOnlyList<SchemaCompilerFinding> Findings, IReadOnlyDictionary<string, double> Metrics);
public sealed class SchemaCompilerEngine
{
    public SchemaCompilerReport Execute(SchemaCompilerInput input, DateTimeOffset now)
    {
        List<SchemaCompilerFinding> findings = new(); Dictionary<string, double> metrics = new(StringComparer.OrdinalIgnoreCase) { ["argumentCount"] = input.Arguments.Count, ["optionCount"] = input.Options.Count, ["uniqueArgumentCount"] = input.Arguments.Distinct(StringComparer.OrdinalIgnoreCase).Count() };
        if (string.IsNullOrWhiteSpace(input.Command)) { findings.Add(new("missing-command", "error", "input", "A command is required.", new Dictionary<string, string>())); }
        foreach (string duplicate in input.Arguments.GroupBy(static item => item, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1).Select(static group => group.Key)) { findings.Add(new("duplicate-argument", "warning", duplicate, "The argument occurs more than once.", new Dictionary<string, string>())); }
        return new("SchemaCompiler", now, now.AddMilliseconds(Math.Max(1, input.Arguments.Count)), findings, metrics);
    }
}
public static class Program
{
    public static int Main(string[] args)
    {
        SchemaCompilerReport report = new SchemaCompilerEngine().Execute(new(args.FirstOrDefault() ?? string.Empty, args.Skip(1).ToArray(), new Dictionary<string, string>()), DateTimeOffset.UtcNow);
        Console.WriteLine(JsonSerializer.Serialize(report, SchemaCompilerJsonContext.Default.SchemaCompilerReport));
        return report.Findings.Any(static finding => finding.Severity == "error") ? 1 : 0;
    }
}
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SchemaCompilerReport))]
internal sealed partial class SchemaCompilerJsonContext : JsonSerializerContext
{
}