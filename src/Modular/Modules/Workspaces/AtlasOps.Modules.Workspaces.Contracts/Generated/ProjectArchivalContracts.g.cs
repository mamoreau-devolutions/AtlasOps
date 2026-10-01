namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectArchivalRecord(Guid Id, string Name, string Owner, ProjectArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectArchivalQuery(string? SearchText, ProjectArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectArchivalPage(IReadOnlyList<ProjectArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectArchivalMutation(bool Succeeded, string Code, string Message, ProjectArchivalRecord? Record, ProjectArchivalEvent? Event);
public interface IProjectArchivalRepository
{
    ValueTask<ProjectArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectArchivalPage> QueryAsync(ProjectArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectArchivalMutation> SaveAsync(ProjectArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectArchivalEventSink { ValueTask PublishAsync(ProjectArchivalEvent domainEvent, CancellationToken cancellationToken); }