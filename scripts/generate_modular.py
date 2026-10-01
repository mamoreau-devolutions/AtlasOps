from __future__ import annotations

import argparse
import json
import os
import subprocess
from pathlib import Path


def write(path: Path, content: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.replace("\r\n", "\n").rstrip("\n").replace("\n", "\r\n"), encoding="utf-8", newline="")


def render(template: str, **values: str) -> str:
    for key, value in values.items():
        template = template.replace("{{" + key + "}}", value)
    return template


def relative_project(source: Path, target: Path) -> str:
    return os.path.relpath(target, source).replace("/", "\\")


def project(path: Path, references: list[Path] | None = None, packages: list[str] | None = None, executable: bool = False, test: bool = False) -> None:
    references = references or []
    packages = packages or []
    properties = ["    <TargetFramework>net10.0-windows</TargetFramework>", "    <ImplicitUsings>enable</ImplicitUsings>", "    <Nullable>enable</Nullable>", "    <LangVersion>preview</LangVersion>"]
    if executable:
        properties.insert(0, "    <OutputType>Exe</OutputType>")
    if test:
        properties.extend(["    <IsPackable>false</IsPackable>", "    <IsTestProject>true</IsTestProject>"])
    groups: list[str] = []
    if packages:
        groups.append("  <ItemGroup>\n" + "\n".join(f'    <PackageReference Include="{item}" />' for item in sorted(set(packages))) + "\n  </ItemGroup>")
    if test:
        groups.append('  <ItemGroup>\n    <Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />\n  </ItemGroup>')
    if references:
        groups.append("  <ItemGroup>\n" + "\n".join(f'    <ProjectReference Include="{relative_project(path.parent, item)}" />' for item in sorted(set(references))) + "\n  </ItemGroup>")
    body = '<Project Sdk="Microsoft.NET.Sdk">\n  <PropertyGroup>\n' + "\n".join(properties) + "\n  </PropertyGroup>\n" + "\n".join(groups) + "\n</Project>\n"
    write(path, body)


PLATFORM = """namespace AtlasOps.{{NAME}};

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record {{NAME}}Descriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record {{NAME}}Operation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record {{NAME}}Outcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface I{{NAME}}Pipeline { ValueTask<{{NAME}}Outcome> ExecuteAsync({{NAME}}Operation operation, CancellationToken cancellationToken); }

public sealed class {{NAME}}Registry
{
    private readonly ConcurrentDictionary<string, {{NAME}}Descriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<{{NAME}}Descriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register({{NAME}}Descriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out {{NAME}}Descriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<{{NAME}}Descriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class {{NAME}}Pipeline : I{{NAME}}Pipeline
{
    private readonly List<Func<{{NAME}}Operation, CancellationToken, ValueTask<{{NAME}}Outcome?>>> _stages = new();
    public void Add(Func<{{NAME}}Operation, CancellationToken, ValueTask<{{NAME}}Outcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<{{NAME}}Outcome> ExecuteAsync({{NAME}}Operation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<{{NAME}}Operation, CancellationToken, ValueTask<{{NAME}}Outcome?>> stage in _stages)
        {
            {{NAME}}Outcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new {{NAME}}Outcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}
"""

CONTRACT = """namespace AtlasOps.Modules.{{DOMAIN}}.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum {{CAPABILITY}}State { Draft, Active, Paused, Completed, Archived }
public sealed record {{CAPABILITY}}Record(Guid Id, string Name, string Owner, {{CAPABILITY}}State State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record {{CAPABILITY}}Command(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record {{CAPABILITY}}Event(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record {{CAPABILITY}}Query(string? SearchText, {{CAPABILITY}}State? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record {{CAPABILITY}}Page(IReadOnlyList<{{CAPABILITY}}Record> Items, int Offset, int Limit, int TotalCount);
public sealed record {{CAPABILITY}}Mutation(bool Succeeded, string Code, string Message, {{CAPABILITY}}Record? Record, {{CAPABILITY}}Event? Event);
public interface I{{CAPABILITY}}Repository
{
    ValueTask<{{CAPABILITY}}Record?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<{{CAPABILITY}}Page> QueryAsync({{CAPABILITY}}Query query, CancellationToken cancellationToken);
    ValueTask<{{CAPABILITY}}Mutation> SaveAsync({{CAPABILITY}}Record record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface I{{CAPABILITY}}EventSink { ValueTask PublishAsync({{CAPABILITY}}Event domainEvent, CancellationToken cancellationToken); }
"""

BEHAVIOR = """namespace AtlasOps.Modules.{{DOMAIN}}.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AtlasOps.Modules.{{DOMAIN}}.Contracts;

public sealed record {{CAPABILITY}}ValidationIssue(string Field, string Code, string Message);
public sealed record {{CAPABILITY}}PolicyDecision(bool Allowed, string Code, string Reason);
public sealed class {{CAPABILITY}}Validator
{
    public IReadOnlyList<{{CAPABILITY}}ValidationIssue> Validate({{CAPABILITY}}Record record)
    {
        List<{{CAPABILITY}}ValidationIssue> issues = new();
        if (record.Id == Guid.Empty) { issues.Add(new(nameof(record.Id), "required", "A stable identifier is required.")); }
        if (string.IsNullOrWhiteSpace(record.Name) || record.Name.Length > 160) { issues.Add(new(nameof(record.Name), "range", "Name must contain between 1 and 160 characters.")); }
        if (string.IsNullOrWhiteSpace(record.Owner) || record.Owner.Length > 120) { issues.Add(new(nameof(record.Owner), "range", "Owner must contain between 1 and 120 characters.")); }
        if (record.Priority is < 0 or > 100) { issues.Add(new(nameof(record.Priority), "range", "Priority must be between 0 and 100.")); }
        if (record.EstimatedCost < 0m) { issues.Add(new(nameof(record.EstimatedCost), "minimum", "Estimated cost cannot be negative.")); }
        if (record.RiskScore is < 0d or > 1d) { issues.Add(new(nameof(record.RiskScore), "range", "Risk score must be between zero and one.")); }
        if (record.UpdatedAt < record.CreatedAt) { issues.Add(new(nameof(record.UpdatedAt), "chronology", "Updated time cannot precede created time.")); }
        if (record.DueAt.HasValue && record.DueAt.Value < record.CreatedAt) { issues.Add(new(nameof(record.DueAt), "chronology", "Due time cannot precede created time.")); }
        return issues;
    }
}
public sealed class {{CAPABILITY}}Policy
{
    private static readonly IReadOnlyDictionary<{{CAPABILITY}}State, IReadOnlySet<string>> Actions = new Dictionary<{{CAPABILITY}}State, IReadOnlySet<string>>
    {
        [{{CAPABILITY}}State.Draft] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "activate", "archive", "update" },
        [{{CAPABILITY}}State.Active] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pause", "complete", "archive", "update" },
        [{{CAPABILITY}}State.Paused] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "resume", "archive", "update" },
        [{{CAPABILITY}}State.Completed] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "reopen", "archive" },
        [{{CAPABILITY}}State.Archived] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "restore" }
    };
    public {{CAPABILITY}}PolicyDecision Evaluate({{CAPABILITY}}Record record, {{CAPABILITY}}Command command)
    {
        if (!Actions[record.State].Contains(command.Action)) { return new(false, "transition-denied", "The action is not valid for the current state."); }
        if (record.RiskScore > 0.85d && !command.Parameters.ContainsKey("approval")) { return new(false, "approval-required", "High-risk records require an approval reference."); }
        if (record.Priority >= 90 && !command.Parameters.ContainsKey("changeTicket")) { return new(false, "ticket-required", "Critical-priority records require a change ticket."); }
        if (command.ExpectedRevision != record.Revision) { return new(false, "revision-conflict", "The record changed after the command was prepared."); }
        return new(true, "allowed", "The requested action satisfies the policy.");
    }
}
public sealed class {{CAPABILITY}}StateMachine
{
    public {{CAPABILITY}}Mutation Apply({{CAPABILITY}}Record current, {{CAPABILITY}}Command command)
    {
        {{CAPABILITY}}State? next = command.Action.ToLowerInvariant() switch
        {
            "activate" or "resume" or "reopen" or "restore" => {{CAPABILITY}}State.Active,
            "pause" => {{CAPABILITY}}State.Paused,
            "complete" => {{CAPABILITY}}State.Completed,
            "archive" => {{CAPABILITY}}State.Archived,
            "update" => current.State,
            _ => null
        };
        if (!next.HasValue) { return new(false, "unknown-action", "The requested action is not recognized.", current, null); }
        string name = command.Parameters.TryGetValue("name", out string? suppliedName) && !string.IsNullOrWhiteSpace(suppliedName) ? suppliedName : current.Name;
        string owner = command.Parameters.TryGetValue("owner", out string? suppliedOwner) && !string.IsNullOrWhiteSpace(suppliedOwner) ? suppliedOwner : current.Owner;
        {{CAPABILITY}}Record updated = current with { Name = name, Owner = owner, State = next.Value, UpdatedAt = command.RequestedAt, Revision = current.Revision + 1 };
        Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase) { ["previousState"] = current.State.ToString(), ["nextState"] = updated.State.ToString(), ["action"] = command.Action };
        {{CAPABILITY}}Event domainEvent = new(Guid.NewGuid(), updated.Id, string.Concat("{{CAPABILITY}}.", command.Action), command.Actor, updated.Revision, command.RequestedAt, details);
        return new(true, "applied", "The state transition was applied.", updated, domainEvent);
    }
}
public sealed class InMemory{{CAPABILITY}}Repository : I{{CAPABILITY}}Repository
{
    private readonly ConcurrentDictionary<Guid, {{CAPABILITY}}Record> _records = new();
    public ValueTask<{{CAPABILITY}}Record?> GetAsync(Guid id, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); _records.TryGetValue(id, out {{CAPABILITY}}Record? record); return ValueTask.FromResult(record); }
    public ValueTask<{{CAPABILITY}}Page> QueryAsync({{CAPABILITY}}Query query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); IEnumerable<{{CAPABILITY}}Record> result = _records.Values;
        if (!string.IsNullOrWhiteSpace(query.SearchText)) { result = result.Where(record => record.Name.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase) || record.Owner.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase)); }
        if (query.State.HasValue) { result = result.Where(record => record.State == query.State.Value); }
        if (!string.IsNullOrWhiteSpace(query.Owner)) { result = result.Where(record => string.Equals(record.Owner, query.Owner, StringComparison.OrdinalIgnoreCase)); }
        if (query.MinimumPriority.HasValue) { result = result.Where(record => record.Priority >= query.MinimumPriority.Value); }
        if (query.MaximumRisk.HasValue) { result = result.Where(record => record.RiskScore <= query.MaximumRisk.Value); }
        {{CAPABILITY}}Record[] ordered = result.OrderByDescending(static record => record.Priority).ThenBy(static record => record.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        int offset = Math.Max(0, query.Offset); int limit = Math.Clamp(query.Limit, 1, 500);
        return ValueTask.FromResult(new {{CAPABILITY}}Page(ordered.Skip(offset).Take(limit).ToArray(), offset, limit, ordered.Length));
    }
    public ValueTask<{{CAPABILITY}}Mutation> SaveAsync({{CAPABILITY}}Record record, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        while (true)
        {
            if (!_records.TryGetValue(record.Id, out {{CAPABILITY}}Record? existing))
            {
                if (expectedRevision != 0) { return ValueTask.FromResult(new {{CAPABILITY}}Mutation(false, "revision-conflict", "A new record must use revision zero.", null, null)); }
                if (_records.TryAdd(record.Id, record)) { return ValueTask.FromResult(new {{CAPABILITY}}Mutation(true, "created", "The record was created.", record, null)); }
                continue;
            }
            if (existing.Revision != expectedRevision) { return ValueTask.FromResult(new {{CAPABILITY}}Mutation(false, "revision-conflict", "The stored revision does not match the expected revision.", existing, null)); }
            if (_records.TryUpdate(record.Id, record, existing)) { return ValueTask.FromResult(new {{CAPABILITY}}Mutation(true, "updated", "The record was updated.", record, null)); }
        }
    }
    public ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_records.TryGetValue(id, out {{CAPABILITY}}Record? existing) || existing.Revision != expectedRevision) { return ValueTask.FromResult(false); }
        return ValueTask.FromResult(_records.TryRemove(new KeyValuePair<Guid, {{CAPABILITY}}Record>(id, existing)));
    }
}
public sealed class Buffering{{CAPABILITY}}EventSink : I{{CAPABILITY}}EventSink
{
    private readonly List<{{CAPABILITY}}Event> _events = new(); private readonly object _sync = new();
    public IReadOnlyList<{{CAPABILITY}}Event> Events { get { lock (_sync) { return _events.ToArray(); } } }
    public ValueTask PublishAsync({{CAPABILITY}}Event domainEvent, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); lock (_sync) { _events.Add(domainEvent); } return ValueTask.CompletedTask; }
}
public sealed class {{CAPABILITY}}Service
{
    private readonly I{{CAPABILITY}}Repository _repository; private readonly I{{CAPABILITY}}EventSink _eventSink;
    private readonly {{CAPABILITY}}Validator _validator = new(); private readonly {{CAPABILITY}}Policy _policy = new(); private readonly {{CAPABILITY}}StateMachine _machine = new();
    public {{CAPABILITY}}Service(I{{CAPABILITY}}Repository repository, I{{CAPABILITY}}EventSink eventSink) { _repository = repository; _eventSink = eventSink; }
    public async ValueTask<{{CAPABILITY}}Mutation> ExecuteAsync({{CAPABILITY}}Command command, CancellationToken cancellationToken)
    {
        {{CAPABILITY}}Record? current = await _repository.GetAsync(command.RecordId, cancellationToken).ConfigureAwait(false);
        if (current is null) { return new(false, "not-found", "The requested record does not exist.", null, null); }
        {{CAPABILITY}}PolicyDecision decision = _policy.Evaluate(current, command);
        if (!decision.Allowed) { return new(false, decision.Code, decision.Reason, current, null); }
        {{CAPABILITY}}Mutation transition = _machine.Apply(current, command);
        if (!transition.Succeeded || transition.Record is null) { return transition; }
        IReadOnlyList<{{CAPABILITY}}ValidationIssue> issues = _validator.Validate(transition.Record);
        if (issues.Count > 0) { return new(false, issues[0].Code, issues[0].Message, current, null); }
        {{CAPABILITY}}Mutation saved = await _repository.SaveAsync(transition.Record, current.Revision, cancellationToken).ConfigureAwait(false);
        if (saved.Succeeded && transition.Event is not null) { await _eventSink.PublishAsync(transition.Event, cancellationToken).ConfigureAwait(false); }
        return saved.Succeeded ? transition : saved;
    }
}
"""

ANALYTICS = """namespace AtlasOps.Modules.{{DOMAIN}}.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.{{DOMAIN}}.Contracts;

public sealed record {{CAPABILITY}}Analytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<{{CAPABILITY}}State, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record Ranked{{CAPABILITY}}({{CAPABILITY}}Record Record, double Score, IReadOnlyList<string> Reasons);
public sealed record {{CAPABILITY}}HistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, {{CAPABILITY}}Record Record);

public sealed class {{CAPABILITY}}AnalyticsEngine
{
    public {{CAPABILITY}}Analytics Analyze(IEnumerable<{{CAPABILITY}}Record> records, DateTimeOffset now)
    {
        {{CAPABILITY}}Record[] snapshot = records.ToArray();
        Dictionary<{{CAPABILITY}}State, int> states = Enum.GetValues<{{CAPABILITY}}State>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == {{CAPABILITY}}State.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not {{CAPABILITY}}State.Completed and not {{CAPABILITY}}State.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class {{CAPABILITY}}RankingEngine
{
    public IReadOnlyList<Ranked{{CAPABILITY}}> Rank(IEnumerable<{{CAPABILITY}}Record> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static Ranked{{CAPABILITY}} RankRecord({{CAPABILITY}}Record record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == {{CAPABILITY}}State.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class {{CAPABILITY}}HistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<{{CAPABILITY}}HistorySnapshot>> _history = new();
    public void Capture({{CAPABILITY}}Record record, DateTimeOffset capturedAt, string reason)
    {
        List<{{CAPABILITY}}HistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<{{CAPABILITY}}HistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<{{CAPABILITY}}HistorySnapshot>? snapshots)) { return Array.Empty<{{CAPABILITY}}HistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public {{CAPABILITY}}Record? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        {{CAPABILITY}}HistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}
"""

VIEW_MODEL = """namespace AtlasOps.Modules.{{DOMAIN}}.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.{{DOMAIN}}.Core;

public sealed class {{DOMAIN}}WorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private {{DOMAIN}}CapabilityDescriptor? _selectedCapability;
    public string Title => {{DOMAIN}}Module.DisplayName;
    public string Summary => string.Concat({{DOMAIN}}Module.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<{{DOMAIN}}CapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? {{DOMAIN}}Module.Capabilities : {{DOMAIN}}Module.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public {{DOMAIN}}CapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}
"""

VIEW = """<local:{{DOMAIN}}WorkbenchView xmlns=\"https://github.com/avaloniaui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"using:AtlasOps.Modules.{{DOMAIN}}.Avalonia\" xmlns:core=\"using:AtlasOps.Modules.{{DOMAIN}}.Core\" x:Class=\"AtlasOps.Modules.{{DOMAIN}}.Avalonia.{{DOMAIN}}WorkbenchView\" x:DataType=\"local:{{DOMAIN}}WorkbenchViewModel\" MinWidth=\"640\" MinHeight=\"420\">
  <Grid Margin=\"24\" RowDefinitions=\"Auto,Auto,Auto,*\" RowSpacing=\"12\">
    <TextBlock FontSize=\"24\" FontWeight=\"SemiBold\" Text=\"{Binding Title}\" />
    <TextBlock Grid.Row=\"1\" Opacity=\"0.72\" Text=\"{Binding Summary}\" />
    <TextBox Grid.Row=\"2\" Text=\"{Binding SearchText}\" PlaceholderText=\"Filter capabilities\" />
    <Grid Grid.Row=\"3\" ColumnDefinitions=\"2*,3*\" ColumnSpacing=\"16\">
      <ListBox ItemsSource=\"{Binding Capabilities}\" SelectedItem=\"{Binding SelectedCapability}\">
        <ListBox.ItemTemplate><DataTemplate x:DataType=\"core:{{DOMAIN}}CapabilityDescriptor\"><StackPanel Margin=\"8\" Spacing=\"3\"><TextBlock FontWeight=\"SemiBold\" Text=\"{Binding DisplayName}\" /><TextBlock FontSize=\"12\" Opacity=\"0.7\" Text=\"{Binding Concern}\" /></StackPanel></DataTemplate></ListBox.ItemTemplate>
      </ListBox>
      <Border Grid.Column=\"1\" Padding=\"20\" Background=\"#0BFFFFFF\" CornerRadius=\"8\"><StackPanel Spacing=\"10\"><TextBlock FontSize=\"20\" FontWeight=\"SemiBold\" Text=\"{Binding SelectedCapability.DisplayName, FallbackValue='Select a capability'}\" /><TextBlock TextWrapping=\"Wrap\" Text=\"{Binding SelectedCapability.Description}\" /><TextBlock Opacity=\"0.7\" Text=\"{Binding SelectedCapability.Id}\" /><TextBlock Opacity=\"0.7\" Text=\"{Binding SelectedCapability.Area}\" /></StackPanel></Border>
    </Grid>
  </Grid>
</local:{{DOMAIN}}WorkbenchView>
"""

VIEW_CODE = """namespace AtlasOps.Modules.{{DOMAIN}}.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class {{DOMAIN}}WorkbenchView : UserControl
{
    public {{DOMAIN}}WorkbenchView() { this.InitializeComponent(); this.DataContext = new {{DOMAIN}}WorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}
"""

ADAPTER = """namespace AtlasOps.Adapters.{{ADAPTER}};

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public sealed record {{CLASS}}Request(string Operation, string Resource, string? ContinuationToken, int PageSize, IReadOnlyDictionary<string, string> Parameters);
public sealed record {{CLASS}}Record(string Id, string Kind, string Region, decimal Cost, DateTimeOffset ObservedAt, string SourceHash, IReadOnlyDictionary<string, string> Properties);
public sealed record {{CLASS}}Page(IReadOnlyList<{{CLASS}}Record> Items, string? ContinuationToken, bool HasMore);
public sealed record {{CLASS}}Health(bool Healthy, string Code, TimeSpan Latency, DateTimeOffset CheckedAt, IReadOnlyDictionary<string, string> Diagnostics);
public interface I{{CLASS}}Transport { ValueTask<{{CLASS}}Page> ExecuteAsync({{CLASS}}Request request, CancellationToken cancellationToken); }
public sealed class {{CLASS}}Normalizer
{
    public {{CLASS}}Record Normalize(string id, string kind, string region, decimal cost, DateTimeOffset observedAt, IReadOnlyDictionary<string, string> properties)
    {
        string normalizedId = id.Trim().ToUpperInvariant(); string normalizedKind = string.IsNullOrWhiteSpace(kind) ? "unknown" : kind.Trim().ToLowerInvariant(); string normalizedRegion = string.IsNullOrWhiteSpace(region) ? "global" : region.Trim().ToLowerInvariant();
        string canonical = string.Join("|", normalizedId, normalizedKind, normalizedRegion, cost.ToString(CultureInfo.InvariantCulture), observedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        return new(normalizedId, normalizedKind, normalizedRegion, Math.Max(0m, cost), observedAt.ToUniversalTime(), hash, new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase));
    }
}
public sealed class {{CLASS}}Policy
{
    public IReadOnlyList<string> Validate({{CLASS}}Request request)
    {
        List<string> issues = new();
        if (string.IsNullOrWhiteSpace(request.Operation)) { issues.Add("Operation is required."); }
        if (string.IsNullOrWhiteSpace(request.Resource)) { issues.Add("Resource is required."); }
        if (request.PageSize is < 1 or > 1000) { issues.Add("Page size must be between 1 and 1000."); }
        return issues;
    }
}
public sealed class InMemory{{CLASS}}Transport : I{{CLASS}}Transport
{
    private readonly ConcurrentDictionary<string, {{CLASS}}Record> _records = new(StringComparer.OrdinalIgnoreCase);
    public void Seed(IEnumerable<{{CLASS}}Record> records) { foreach ({{CLASS}}Record record in records) { _records[record.Id] = record; } }
    public ValueTask<{{CLASS}}Page> ExecuteAsync({{CLASS}}Request request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); int offset = int.TryParse(request.ContinuationToken, NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
        {{CLASS}}Record[] filtered = _records.Values.Where(record => record.Kind.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Region.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Id.Contains(request.Resource, StringComparison.OrdinalIgnoreCase)).OrderBy(static record => record.Id, StringComparer.OrdinalIgnoreCase).ToArray();
        {{CLASS}}Record[] page = filtered.Skip(offset).Take(request.PageSize).ToArray(); int nextOffset = offset + page.Length; string? token = nextOffset < filtered.Length ? nextOffset.ToString(CultureInfo.InvariantCulture) : null;
        return ValueTask.FromResult(new {{CLASS}}Page(page, token, token is not null));
    }
}
public sealed class {{CLASS}}Adapter
{
    private readonly I{{CLASS}}Transport _transport; private readonly {{CLASS}}Policy _policy = new();
    public {{CLASS}}Adapter(I{{CLASS}}Transport transport) { _transport = transport; }
    public async ValueTask<{{CLASS}}Page> QueryAsync({{CLASS}}Request request, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> issues = _policy.Validate(request); if (issues.Count > 0) { return new(Array.Empty<{{CLASS}}Record>(), null, false); }
        return await _transport.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
    }
    public async ValueTask<{{CLASS}}Health> ProbeAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        try { await _transport.ExecuteAsync(new("health", string.Empty, null, 1, new Dictionary<string, string>()), cancellationToken).ConfigureAwait(false); return new(true, "ready", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string> { ["adapter"] = "{{ADAPTER}}" }); }
        catch (OperationCanceledException) { return new(false, "cancelled", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string>()); }
    }
}
"""

TOOL = """namespace AtlasOps.Tools.{{TOOL}};

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public sealed record {{TOOL}}Input(string Command, IReadOnlyList<string> Arguments, IReadOnlyDictionary<string, string> Options);
public sealed record {{TOOL}}Finding(string Code, string Severity, string Subject, string Message, IReadOnlyDictionary<string, string> Evidence);
public sealed record {{TOOL}}Report(string Tool, DateTimeOffset StartedAt, DateTimeOffset CompletedAt, IReadOnlyList<{{TOOL}}Finding> Findings, IReadOnlyDictionary<string, double> Metrics);
public sealed class {{TOOL}}Engine
{
    public {{TOOL}}Report Execute({{TOOL}}Input input, DateTimeOffset now)
    {
        List<{{TOOL}}Finding> findings = new(); Dictionary<string, double> metrics = new(StringComparer.OrdinalIgnoreCase) { ["argumentCount"] = input.Arguments.Count, ["optionCount"] = input.Options.Count, ["uniqueArgumentCount"] = input.Arguments.Distinct(StringComparer.OrdinalIgnoreCase).Count() };
        if (string.IsNullOrWhiteSpace(input.Command)) { findings.Add(new("missing-command", "error", "input", "A command is required.", new Dictionary<string, string>())); }
        foreach (string duplicate in input.Arguments.GroupBy(static item => item, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1).Select(static group => group.Key)) { findings.Add(new("duplicate-argument", "warning", duplicate, "The argument occurs more than once.", new Dictionary<string, string>())); }
        return new("{{TOOL}}", now, now.AddMilliseconds(Math.Max(1, input.Arguments.Count)), findings, metrics);
    }
}
public static class Program
{
    public static int Main(string[] args)
    {
        {{TOOL}}Report report = new {{TOOL}}Engine().Execute(new(args.FirstOrDefault() ?? string.Empty, args.Skip(1).ToArray(), new Dictionary<string, string>()), DateTimeOffset.UtcNow);
        Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        return report.Findings.Any(static finding => finding.Severity == "error") ? 1 : 0;
    }
}
"""

def domain_catalog(domain: dict) -> str:
    entries = []
    for noun in domain["nouns"]:
        for concern in domain["concerns"]:
            capability = noun + concern
            entries.append(f'        new("{domain["id"]}.{capability}", "{noun} {concern.lower()}", "{noun}", "{concern}", "Coordinates {domain["display"].lower()} for {noun} {concern.lower()}."),')
    return f'''namespace AtlasOps.Modules.{domain["id"]}.Core;\n\nusing System.Collections.Generic;\n\npublic sealed record {domain["id"]}CapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);\npublic static class {domain["id"]}Module\n{{\n    public const string Id = "{domain["id"]}";\n    public const string DisplayName = "{domain["display"]}";\n    public static IReadOnlyList<{domain["id"]}CapabilityDescriptor> Capabilities {{ get; }} = new {domain["id"]}CapabilityDescriptor[]\n    {{\n''' + "\n".join(entries) + "\n    };\n}\n"


def composition_source(domains: list[dict]) -> str:
    usings = "\n".join(f"using AtlasOps.Modules.{item['id']}.Avalonia;" for item in domains)
    enterprise_views = {
        "Automation": "WorkflowStudioView",
        "Governance": "PolicyStudioView",
        "Incidents": "IncidentStudioView",
        "Inventory": "InventoryStudioView",
        "Observability": "ReportingStudioView",
    }
    entries = "\n".join(
        f'        new("{item["id"]}", "{item["display"]}", {item["id"]}Module.Capabilities.Count, static () => new {enterprise_views.get(item["id"], item["id"] + "WorkbenchView")}()),'
        for item in domains
    )
    core_usings = "\n".join(f"using AtlasOps.Modules.{item['id']}.Core;" for item in domains)
    enterprise_usings = "\n".join(
        [
            "using AtlasOps.Enterprise.Avalonia.Incidents;",
            "using AtlasOps.Enterprise.Avalonia.Inventory;",
            "using AtlasOps.Enterprise.Avalonia.Policy;",
            "using AtlasOps.Enterprise.Avalonia.Reporting;",
            "using AtlasOps.Enterprise.Avalonia.Workflow;",
        ]
    )
    return f'''namespace AtlasOps.Composition;\n\nusing System;\nusing System.Collections.Generic;\n\nusing global::Avalonia.Controls;\n\n{enterprise_usings}\n{usings}\n{core_usings}\n\npublic sealed record AtlasOpsModuleDescriptor(string Id, string DisplayName, int CapabilityCount, Func<UserControl> CreateView);\npublic static class AtlasOpsModuleCatalog\n{{\n    public static IReadOnlyList<AtlasOpsModuleDescriptor> Create()\n    {{\n        return new AtlasOpsModuleDescriptor[]\n        {{\n{entries}\n        }};\n    }}\n}}\n'''


def generate(root: Path, skip_solution: bool) -> None:
    manifest = json.loads((root / "config" / "atlasops-modules.json").read_text(encoding="utf-8-sig"))
    generated_root = root / "src" / "Modular"
    test_root = root / "tests" / "Modular"
    generated_root.mkdir(parents=True, exist_ok=True)
    test_root.mkdir(parents=True, exist_ok=True)
    paths: list[Path] = []
    platform: dict[str, Path] = {}
    platform_packages = {
        "Scheduling": ["Ical.Net", "Quartz"],
        "Security": ["BouncyCastle.Cryptography", "PgpCore", "Sodium.Core", "SSH.NET", "System.IdentityModel.Tokens.Jwt", "VaultSharp", "Yubico.YubiKey"],
        "Serialization": ["CsvHelper", "DocumentFormat.OpenXml", "MessagePack", "protobuf-net", "Scriban", "SharpCompress", "YamlDotNet"],
        "Telemetry": ["Serilog"],
    }
    for name in manifest["platformProjects"]:
        path = generated_root / "Platform" / f"AtlasOps.{name}" / f"AtlasOps.{name}.csproj"
        packages = ["Avalonia"] if name in {"Composition", "Shell", "DesignSystem"} else platform_packages.get(name, [])
        project(path, packages=packages)
        write(path.parent / f"{name}Primitives.g.cs", render(PLATFORM, NAME=name))
        platform[name] = path
        paths.append(path)

    domain_projects: dict[str, dict[str, Path]] = {}
    for domain in manifest["domains"]:
        domain_id = domain["id"]
        base = generated_root / "Modules" / domain_id
        contracts = base / f"AtlasOps.Modules.{domain_id}.Contracts" / f"AtlasOps.Modules.{domain_id}.Contracts.csproj"
        core = base / f"AtlasOps.Modules.{domain_id}.Core" / f"AtlasOps.Modules.{domain_id}.Core.csproj"
        avalonia = base / f"AtlasOps.Modules.{domain_id}.Avalonia" / f"AtlasOps.Modules.{domain_id}.Avalonia.csproj"
        project(contracts)
        project(core, [contracts, platform["Domain"], platform["Validation"], platform["Application"]])
        project(avalonia, [contracts, core, platform["DesignSystem"]], ["Avalonia"])
        for noun in domain["nouns"]:
            for concern in domain["concerns"]:
                capability = noun + concern
                write(contracts.parent / "Generated" / f"{capability}Contracts.g.cs", render(CONTRACT, DOMAIN=domain_id, CAPABILITY=capability))
                write(core.parent / "Generated" / f"{capability}Behavior.g.cs", render(BEHAVIOR, DOMAIN=domain_id, CAPABILITY=capability))
                write(core.parent / "Generated" / f"{capability}Analytics.g.cs", render(ANALYTICS, DOMAIN=domain_id, CAPABILITY=capability))
        write(core.parent / f"{domain_id}Module.g.cs", domain_catalog(domain))
        write(avalonia.parent / f"{domain_id}WorkbenchViewModel.g.cs", render(VIEW_MODEL, DOMAIN=domain_id))
        write(avalonia.parent / f"{domain_id}WorkbenchView.axaml", render(VIEW, DOMAIN=domain_id))
        write(avalonia.parent / f"{domain_id}WorkbenchView.axaml.cs", render(VIEW_CODE, DOMAIN=domain_id))
        domain_projects[domain_id] = {"contracts": contracts, "core": core, "avalonia": avalonia}
        paths.extend([contracts, core, avalonia])

    enterprise_avalonia = root / "src" / "Enterprise" / "AtlasOps.Enterprise.Avalonia" / "AtlasOps.Enterprise.Avalonia.csproj"
    project(platform["Composition"], [* [item["avalonia"] for item in domain_projects.values()], enterprise_avalonia], ["Avalonia"])
    write(platform["Composition"].parent / "AtlasOpsModuleCatalog.g.cs", composition_source(manifest["domains"]))

    adapter_paths: dict[str, Path] = {}
    adapter_packages = {
        "Aws": ["AWSSDK.Core", "AWSSDK.EC2", "AWSSDK.IdentityManagement", "AWSSDK.Route53", "AWSSDK.S3", "AWSSDK.SSO", "AWSSDK.SSOOIDC"],
        "Azure": ["Azure.Identity", "Azure.ResourceManager.Authorization", "Azure.Security.KeyVault.Secrets", "Azure.Storage.Blobs"],
        "Gcp": ["Google.Apis.Auth"],
        "Persistence.Sqlite": ["Microsoft.Data.Sqlite"],
    }
    for adapter in manifest["adapters"]:
        name = f"AtlasOps.Adapters.{adapter}"
        path = generated_root / "Adapters" / name / f"{name}.csproj"
        project(
            path,
            [platform["Configuration"], platform["Security"], platform["Telemetry"]],
            adapter_packages.get(adapter, []))
        class_name = adapter.replace(".", "") + "Source"
        write(path.parent / f"{class_name}.g.cs", render(ADAPTER, ADAPTER=adapter, CLASS=class_name))
        adapter_paths[adapter] = path
        paths.append(path)

    for tool in manifest["tools"]:
        name = f"AtlasOps.Tools.{tool}"
        path = generated_root / "Tools" / name / f"{name}.csproj"
        packages = ["Microsoft.Data.SqlClient", "MongoDB.Driver", "MySqlConnector", "Npgsql", "OpenSearch.Client"] if tool == "Migration" else []
        project(
            path,
            [platform["Configuration"], platform["Domain"], platform["Serialization"], platform["Telemetry"]],
            packages,
            executable=True)
        write(path.parent / "Program.g.cs", render(TOOL, TOOL=tool))
        paths.append(path)

    test_refs = {
        "Architecture": [platform["Composition"], platform["Domain"]],
        "Domain": [domain_projects["Workspaces"]["contracts"], domain_projects["Workspaces"]["core"], domain_projects["Incidents"]["contracts"], domain_projects["Incidents"]["core"]],
        "Application": [platform["Application"], domain_projects["Automation"]["contracts"], domain_projects["Automation"]["core"]],
        "Persistence": [adapter_paths["Persistence.Local"], adapter_paths["Persistence.Sqlite"], adapter_paths["Persistence.Turso"]],
        "Adapters": [
            adapter_paths["Aws"],
            adapter_paths["Azure"],
            adapter_paths["Gcp"],
            adapter_paths["Iana"],
            adapter_paths["Persistence.Sqlite"],
            platform["Scheduling"],
            platform["Security"],
            platform["Serialization"],
            platform["Telemetry"],
            generated_root / "Tools" / "AtlasOps.Tools.Migration" / "AtlasOps.Tools.Migration.csproj",
        ],
        "ReferenceData": [domain_projects["Geography"]["core"], domain_projects["Lifecycle"]["core"], domain_projects["Geospatial"]["core"]],
        "Avalonia": [domain_projects["Workspaces"]["avalonia"], domain_projects["CloudEconomics"]["avalonia"]],
        "Generators": [platform["Serialization"]],
        "EndToEnd": [platform["Composition"], domain_projects["Deployments"]["core"], domain_projects["Governance"]["core"]],
    }
    for test_name in manifest["testProjects"]:
        name = f"AtlasOps.{test_name}.Tests"
        path = test_root / name / f"{name}.csproj"
        packages = ["MSTest", "Avalonia.Headless"] if test_name == "EndToEnd" else ["MSTest"]
        project(path, test_refs[test_name], packages, test=True)
        write(path.parent / "GeneratedProjectMarker.g.cs", f"namespace {name};\n\npublic static class GeneratedProjectMarker\n{{\n    public const string Name = \"{name}\";\n}}\n")
        paths.append(path)

    if len(paths) != 93:
        raise RuntimeError(f"Expected 93 generated projects but produced {len(paths)}.")
    if not skip_solution:
        solution = root / "AtlasOps.sln"
        listed = subprocess.run(["dotnet", "sln", str(solution), "list"], check=True, text=True, capture_output=True).stdout
        listed_paths = {line.strip().replace("/", "\\") for line in listed.splitlines() if line.strip().endswith(".csproj")}
        for path in paths:
            relative = relative_project(root, path)
            if relative not in listed_paths:
                subprocess.run(["dotnet", "sln", str(solution), "add", str(path), "--include-references", "false"], check=True, stdout=subprocess.DEVNULL)
    print(f"Generated {len(paths)} modular projects across {len(manifest['domains'])} domains.")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repository-root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--skip-solution-update", action="store_true")
    args = parser.parse_args()
    generate(args.repository_root.resolve(), args.skip_solution_update)


if __name__ == "__main__":
    main()
